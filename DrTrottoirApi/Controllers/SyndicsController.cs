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
    public class SyndicsController : ControllerBase
    {
        private readonly ISyndicRepository _syndicRepository;

        public SyndicsController(ISyndicRepository syndicRepository)
        {
            _syndicRepository = syndicRepository;
        }

        /// <summary>
        /// Gets all syndics. Authorized for: Admin, SuperStudent.
        /// </summary>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<IList<Syndic>>> Get()
        {
            try
            {
                var syndics = await _syndicRepository.GetAllSyndics();
                return Ok(syndics);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Gets a syndic by id. Authorized for: Admin, Superstudent.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Syndic>> Get(Guid id)
        {
            try
            {
                var syndic = await _syndicRepository.GetSyndicById(id);
                return Ok(syndic);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Creates a syndic. Authorized for: Admin.
        /// </summary>
        /// <param name="syndic"></param>
        /// <returns>ActionResult: Creating the syndic succeeded or not and returns the syndic id</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateSyndicRequest request)
        {
            try
            {
                var result = await _syndicRepository.CreateSyndic(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Deletes a syndic. Authorized for: Admin.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _syndicRepository.DeleteSyndic(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Updates a syndic. Authorized for: Admin.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="syndic"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, CreateSyndicRequest syndic)
        {
            try
            {
                await _syndicRepository.UpdateSyndic(id, syndic);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
