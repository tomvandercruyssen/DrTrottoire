using System.Net;
using DrTrottoirApi.Attributes;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Extensions;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrTrottoirApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        /// <summary>
        /// Authenticates a user with login credentials. Authorized for: All
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <returns>AuthenticateResponse with minimal User info included</returns>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest authenticateRequest)
        {
            try
            {
                var response = await _userRepository.Authenticate(authenticateRequest, GetIpAddress());
                SetTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Renews the token of the user for a longer session. Authorized for: All
        /// </summary>
        /// <returns>AuthenticateResponse with minimal User info included</returns>
        [AllowAnonymous]
        [HttpPost("RenewToken")]
        public async Task<ActionResult<AuthenticateResponse>> RenewToken()
        {
            try
            {
                string refreshToken = Request.Cookies["DrTrottoir.RefreshToken"];

                var response = await _userRepository.RenewToken(refreshToken, GetIpAddress());
                SetTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Deactive a token so user can no longer have acces. Authorized for: All
        /// </summary>
        /// <param name="deactivateTokenRequest"></param>
        /// <returns>Returns ActionResult if the action was handled correctly</returns>
        [AllowAnonymous]
        [HttpPost("DeactivateToken")]
        public async Task<IActionResult> DeactivateToken(DeactivateTokenRequest deactivateTokenRequest)
        {
            try
            {
                string token = deactivateTokenRequest.Token ?? Request.Cookies["DrTrottoir.RefreshToken"];

                if (string.IsNullOrEmpty(token))
                    throw new TokenExpiredException();

                await _userRepository.DeactivateToken(token, GetIpAddress());

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Creates a user. Authorized for: Admin
        /// </summary>
        /// <param name="user"></param>
        /// <returns>ActionResult: Creating the user succeeded or not and the id of the created user</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateUserRequest user)
        {
            try
            {
                var userid = await _userRepository.CreateUser(user);
                return Ok(userid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Get all users with minimal info. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <returns>A List of BaseUserResponses</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<IList<BaseUserResponse>>> Get()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets a user with minimal info. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BaseUserResponse</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseUserResponse>> Get(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Deletes a user with a given id. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ActionResult: Deleting the user succeeded or not</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userRepository.DeleteUserById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Update a user. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns>ActionResult: Updating the user succeeded or not</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, BaseUserRequest user)
        {
            try
            {
                await _userRepository.UpdateUser(id, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets an average time of the student per round. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of Rounds with their timeRecords from the user</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("{id:guid}/TimeRecords")]
        public async Task<IActionResult> GetTimeRecordsOfStudent(Guid id)
        {
            try
            {
                var timeRecords = await _userRepository.GetTimeRecordsOfStudent(id);
                return Ok(timeRecords);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all rounds of a user in the current week. Authorized for: Admin, SuperStudent, Student
        /// </summary>
        /// <param name="userid"></param>
        /// <returns>BaseUserResponse</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent, Roles.Student)]
        [HttpGet("{id:guid}/round")]
        public async Task<ActionResult<IList<Round>>> GetRoundsByUser(Guid id)
        {
            try
            {
                var rounds = await _userRepository.GetRoundsByUserId(id);
                return Ok(rounds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all users for a certain role. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Gets the user by role</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("role")]
        public async Task<IActionResult> GetUsersByRole(Roles role)
        {
            try
            {
                var users = await _userRepository.GetUsersByRole(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private void SetTokenCookie(string token)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(5),
                IsEssential = true
            };

            Response.Cookies.Append("DrTrottoir.RefreshToken", token, cookieOptions);
        }
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

    }
}
