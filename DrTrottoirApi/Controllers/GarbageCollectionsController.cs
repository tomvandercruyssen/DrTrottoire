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
    public class GarbageCollectionsController : ControllerBase
    {
        private readonly IGarbageCollectionRepository _garbageCollectionRepository;

        public GarbageCollectionsController(IGarbageCollectionRepository garbageCollectionRepository)
        {
            _garbageCollectionRepository = garbageCollectionRepository;
        }
        /// <summary>
        /// Create a new GarbageCollection for a company. Authorized for: Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateGarbageCollectionRequest request)
        {
            try
            {
                await _garbageCollectionRepository.CreateGarbageCollection(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Delete a GarbageCollection for a company. Authorized for: Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]DeleteGarbageCollectionRequest request)
        {
            try
            {
                await _garbageCollectionRepository.DeleteGarbageCollection(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all the garbageCollections for that week, for the given company. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("Week")]
        public async Task<IActionResult> GetForWeek([FromQuery]GetGarbageCollectionRequest request)
        {
            try
            {
                var result = await _garbageCollectionRepository.GetGarbageCollectionsWithGarbageTypesForWeek(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets the garbageTypes for a certain garbageCollection in a timeSlot. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet("TimeSlot")]
        public async Task<IActionResult> GetForTimeSlot([FromQuery]GetGarbageCollectionRequest request)
        {
            try
            {
                var result = await _garbageCollectionRepository.GetGarbageCollectionsWithGarbageTypesForTimeSlot(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
