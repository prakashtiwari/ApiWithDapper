using DapperIntegration.Dto;
using DapperIntegration.Entities;

namespace DapperIntegration.Contracts
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetCompanies();
        Task<Company> GetCompany(int id);
        Task<Company> CreateCompany(CompanyDto company);
        Task UpdateCompany(int id, CompanyUpdateDto companyUpdateDto);
        Task DeleteCompany(int id);

        Task<Company> GetCompanyByEmployeeId(int id);
        public Task<Company> GetMultipleResults(int id);

        public Task<List<Company>> GetMultipleMapping();
    }
}
