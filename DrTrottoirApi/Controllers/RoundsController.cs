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
    public class RoundsController : ControllerBase
    {
        private readonly IRoundRepository _roundRepository;

        public RoundsController(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository;
        }

        /// <summary>
        /// Get all rounds out of the database. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <returns>Round objects</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<IList<BaseRoundResponse>>> Get()
        {
            try
            {
                var rounds = await _roundRepository.GetAllRounds();
                return Ok(rounds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets a round with minimal info filtered by id. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BaseUserResponse</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseRoundResponse>> Get(Guid id)
        {
            try
            {
                var round = await _roundRepository.GetRoundById(id);
                return round;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Creates a round. Authorized for: Admin
        /// </summary>
        /// <param name="round"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateRoundRequest request)
        {
            try
            {
                var id = await _roundRepository.AddRound(request);
                return Ok(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Update an existing round. Authorized for: Admin
        /// </summary>
        /// <param name="round"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Put(Guid id, Round round)
        {
            try
            {
                await _roundRepository.UpdateRound(id, round);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Gets the percentage of the finished tasks of the round.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent, Roles.Student)]
        [HttpGet("{id:guid}/progress")]
        public async Task<IActionResult> GetRoundProgress(Guid id)
        {
            try
            {
                var percentage = await _roundRepository.GetProgressOfRound(id);
                return Ok(percentage);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Gets all companies inside a round. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="roundId"></param>
        /// <returns>List of companies</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}/companies")]
        public async Task<ActionResult<IList<Company>>> GetByRound(Guid id)
        {
            try
            {
                var companies = await _roundRepository.GetCompaniesByRound(id);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets all the remarks written in the round. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A List of remarks and companies</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}/remarks")]
        public async Task<IActionResult> GetRemarksOfRound(Guid id)
        {
            try
            {
                var remarks = await _roundRepository.GetRemarksOfRound(id);
                return Ok(remarks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets the tasks for a round (for rounds page). Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, SuperStudent")]
        [HttpGet("{id:guid}/tasks")]
        public async Task<ActionResult> GetTasksForRound(Guid id)
        {
            try
            {
                var tasks = await _roundRepository.GetTasksForRound(id);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Starts a round. Authorized for: SuperStudent, Student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.SuperStudent, Roles.Student)]
        [MobileApp]
        [HttpPost("{id:guid}/start")]
        public async Task<ActionResult> StartRound(Guid id)
        {
            try
            {
                await _roundRepository.StartRound(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Delete a round out of the database. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _roundRepository.DeleteRound(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
