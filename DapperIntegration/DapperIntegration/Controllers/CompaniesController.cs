using DapperIntegration.Contracts;
using DapperIntegration.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperIntegration.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        public CompaniesController(ICompanyRepository companyRepository) => _companyRepository = companyRepository;
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepository.GetCompanies();
            return Ok(companies);

        }
        [HttpGet("{id}", Name = "GetCompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _companyRepository.GetCompany(id);
            if (company == null)
                return NotFound();
            return Ok(company);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            var createdComp = _companyRepository.CreateCompany(companyDto);
            return CreatedAtRoute("GetCompanyById", new { id = createdComp.Id }, createdComp);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyUpdateDto companyUpdateDto)
        {
            var res = await _companyRepository.GetCompany(id);
            if (res == null)
                return NotFound();
            await _companyRepository.UpdateCompany(id, companyUpdateDto);
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _companyRepository.GetCompany(id);
            if (res == null)
                return NotFound();
            await _companyRepository.DeleteCompany(id);
            return NoContent();

        }
        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyByEmpId(int id)
        {

            var result = await _companyRepository.GetCompanyByEmployeeId(id);
            return Ok(result);
        }
        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var result = await _companyRepository.GetMultipleResults(id);

            if (result == null)
                return NoContent();
            return Ok(result);
        }
        [HttpGet("GetMultipleMapping")]
        public async Task<IActionResult> GetMultipleMapping()
        {
            var result = await _companyRepository.GetMultipleMapping();          
            return Ok(result);
        }
    }
}

