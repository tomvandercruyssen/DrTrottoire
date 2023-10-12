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
    public class WorkAreasController : ControllerBase
    {
        private readonly IWorkAreaRepository _workAreaRepository;

        public WorkAreasController(IWorkAreaRepository workAreaRepository)
        {
            _workAreaRepository = workAreaRepository;
        }
        /// <summary>
        /// Gets all the workAreas. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <returns>A List of workAreas</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var workAreas = await _workAreaRepository.GetAllWorkAreas();
                return Ok(workAreas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Gets the workArea by id. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The workArea</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var workArea = await _workAreaRepository.GetWorkAreasById(id);
                return Ok(workArea);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Creates a workArea. Authorized for: Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateWorkAreaRequest request)
        {
            try
            {
                var workArea = await _workAreaRepository.CreateWorkArea(request);
                return Ok(workArea);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Deletes a workArea. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _workAreaRepository.DeleteWorkArea(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Updates a workArea. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, CreateWorkAreaRequest request)
        {
            try
            {
                await _workAreaRepository.UpdateWorkArea(id, request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Returns the users for a certain workArea. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A List of users in the same workArea</returns>
        [Authorize(Roles = "Admin, SuperStudent")]
        [HttpGet("{id:guid}/users")]
        public async Task<IActionResult> GetByWorkArea(Guid id)
        {
            try
            {
                var user = await _workAreaRepository.GetUsersByWorkArea(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
