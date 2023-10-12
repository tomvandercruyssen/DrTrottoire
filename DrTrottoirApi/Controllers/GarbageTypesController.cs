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
    public class GarbageTypesController : ControllerBase
    {
        private readonly IGarbageTypeRepository _garbageTypeRepository;

        public GarbageTypesController(IGarbageTypeRepository garbageTypeRepository)
        {
            _garbageTypeRepository = garbageTypeRepository;
        }

        /// <summary>
        /// Gets All Garbage types.  Authorized For: Admin
        /// </summary>
        /// <returns>A list of garbageTypes</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<List<GarbageTypeResponse>>> Get()
        {
            try
            {
                var users = await _garbageTypeRepository.GetAllGarbageTypes();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a garbageType (ex: PMD, REST, ...). Authorized For: Admin
        /// </summary>
        /// <param name="request">Holds the Name of the garbageType to create</param>
        /// <returns>ActionResult: Creating the garbageType succeeded or not</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateGarbageTypeRequest request)
        {
            try
            {
                await _garbageTypeRepository.CreateGarbageType(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
