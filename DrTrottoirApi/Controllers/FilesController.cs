using DrTrottoirApi.Attributes;
using DrTrottoirApi.CloudStorage;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrTrottoirApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ICloudStorage _cloudStorage;

        public FilesController(ICloudStorage iCloudStorage)
        {
            _cloudStorage = iCloudStorage;
        }
        /// <summary>
        /// Upload an Image for a specific task. Authorized for: Student, SuperStudent, Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent, Roles.Student)]
        [HttpPost("Task/Image")]
        public async Task<IActionResult> PostTaskImage([FromForm]UploadTaskImageRequest request)
        {
            try
            {
                await _cloudStorage.UploadTaskImage(request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Upload an Image for a company. Authorized for: Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost("Company/Image")]
        public async Task<IActionResult> PostCompanyImage([FromForm]UploadCompanyImageRequest request)
        {
            try
            {
                await _cloudStorage.UploadCompanyImage(request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes the appearance Image of the company. Authorized for: Admin
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("Company/Image")]
        public async Task<IActionResult> DeleteCompanyImage(Guid companyId)
        {
            try
            {
                await _cloudStorage.DeleteCompanyImage(companyId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Adds the Manual of the company. Authorized for: Admin
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost("Company/Manual")]
        public async Task<IActionResult> PostCompanyManual([FromForm] UploadCompanyManualRequest request)
        {
            try
            {
                await _cloudStorage.UploadManual(request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
