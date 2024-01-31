using Dapper;
using DapperIntegration.Context;
using DapperIntegration.Contracts;
using DapperIntegration.Dto;
using DapperIntegration.Entities;
using System.Data;

namespace DapperIntegration.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext context;
        public CompanyRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            string query = "Select *from Companies";
            using (var con = context.CreateConnection())
            {
                var companies = await con.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }
        public async Task<Company> GetCompany(int id)
        {
            string query = "Select *from Companies where Id=@Id";

            using (var con = context.CreateConnection())
            {
                var company = con.QuerySingleOrDefault<Company>(query, new { id });
                return company;
            }
        }
        public async Task<Company> CreateCompany(CompanyDto company)
        {
            string query = "Insert into Companies(Name,Address,Country) values(@Name,@Address,@Country)" +
                "Select CAST(SCOPE_IDENTITY() AS int)";
            var dynParam = new DynamicParameters();
            dynParam.Add("Name", company.Name, DbType.String);
            dynParam.Add("Address", company.Address, DbType.String);
            dynParam.Add("Country", company.Country, DbType.String);
            using (var con = context.CreateConnection())
            {
                var result = await con.QuerySingleAsync<int>(query, dynParam);
                var comp = new Company()
                {
                    Id = result,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
                return comp;
            }
        }
        public async Task UpdateCompany(int id, CompanyUpdateDto company)
        {
            string query = "Update Companies set Name=@Name,Address=@Address,Country=@Country where Id=@Id";

            var dynParam = new DynamicParameters();
            dynParam.Add("Id", id, DbType.Int64);
            dynParam.Add("Name", company.Name, DbType.String);
            dynParam.Add("Address", company.Address, DbType.String);
            dynParam.Add("Country", company.Country, DbType.String);
            using (var con = context.CreateConnection())
            {
                var result = await con.ExecuteAsync(query, dynParam);
                var comp = new Company()
                {
                    Id = result,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
            }
        }

        public async Task DeleteCompany(int id)
        {
            string query = "Delete from Companies Where Id=@Id";
            var dynParam = new DynamicParameters();
            dynParam.Add("Id", id, DbType.Int64);
            using (var con = context.CreateConnection())
            {
                await con.ExecuteAsync(query, dynParam);

            }
        }
        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            string procName = "sp_ShowCompanyByEmpId";
            using (var con = context.CreateConnection())
            {
                var dparams = new DynamicParameters();
                dparams.Add("Id", id, DbType.Int64, ParameterDirection.Input);
                Company result = await con.QueryFirstOrDefaultAsync<Company>(procName, dparams, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<Company> GetMultipleResults(int id)
        {
            string query = "Select *from Companies Where Id=@Id;"
            + "Select *from Employees Where CompanyId=@Id";
            using (var con = this.context.CreateConnection())
            using (var multi = await con.QueryMultipleAsync(query, new { id }))
            {
                var comp = await multi.ReadSingleOrDefaultAsync<Company>();
                if (comp != null)
                    comp.Employees = (await multi.ReadAsync<Employee>()).ToList();
                return comp;
            }
        }
        public async Task<List<Company>> GetMultipleMapping()
        {
            string query = "Select *from Companies c inner join Employees e on e.CompanyId=c.Id";

            using (var con = context.CreateConnection())
            {
                var resultDict = new Dictionary<int, Company>();
                var result = await con.QueryAsync<Company, Employee, Company>(
                    query,
                    (company, employee) =>
                    {
                        if (!resultDict.TryGetValue(company.Id, out var currentCpmp))
                        {
                            currentCpmp = company;
                            resultDict.Add(currentCpmp.Id, currentCpmp);
                        }
                        currentCpmp.Employees.Add(employee);
                        return currentCpmp;
                    }


                    );
                return result.Distinct().ToList();
            }
        }
    }
}
