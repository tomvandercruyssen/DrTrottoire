using DrTrottoirApi.Attributes;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrTrottoirApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        /// <summary>
        /// Get all companies out of the database. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <returns>Companies objects</returns>

        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<IList<Company>>> Get()
        {
            try
            {
                var companies = await _companyRepository.GetAllCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a specific company. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Company</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Company>> Get(Guid id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyById(id);
                return company;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Creates a company. Authorized for: Admin
        /// </summary>
        /// <param name="company"></param>
        /// <returns>ActionResult: Creating the company succeeded or not and the company id</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateCompanyRequest company)
        {
            try
            {
                var id = await _companyRepository.AddCompany(company);
                return Ok(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        /// <summary>
        /// Update an existing company. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="company"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, CreateCompanyRequest company)
        {
            try
            {
                await _companyRepository.UpdateCompany(id, company);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Delete a company out of the database. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            try
            {
                await _companyRepository.DeleteCompany(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Gets the coordinates of a company. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}/coordinates")]
        public async Task<IActionResult> GetCoordinates(Guid id)
        {
            try
            {
                var coordinates = await _companyRepository.GetCoordinates(id);
                return Ok(coordinates);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
