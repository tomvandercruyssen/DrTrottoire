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
    public class TasksController: ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        /// <summary>
        /// Get all tasks out of the database. Authorized for: Admin, SuperStudent
        /// </summary>
        /// <returns>Tasks objects</returns>

        [AuthorizedFor(Roles.Admin, Roles.SuperStudent)]
        [HttpGet]
        public async Task<ActionResult<IList<BaseTaskResponse>>> Get()
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasks();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a specific task. Authorized for: Admin, SuperStudent, Student
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task</returns>
        [AuthorizedFor(Roles.Admin, Roles.SuperStudent, Roles.Student)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseTaskResponse>> Get(Guid id)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(id);
                return task;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Creates a task. Authorized for: Admin
        /// </summary>
        /// <param name="task"></param>
        /// <returns>ActionResult: Creating the task succeeded or not and returns the task id</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateTaskRequest task)
        {
            try
            {
                var result = await _taskRepository.AddTask(task);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Update an existing task. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="task"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, CreateTaskRequest task)
        {
            try
            {
                await _taskRepository.UpdateTask(id, task);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Complete a task. Authorized for: SuperStudent, Student
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.SuperStudent, Roles.Student)]
        [MobileApp]
        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            try
            {
                await _taskRepository.CompleteTask(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

        }

        /// <summary>
        /// Delete a task out of the database. Authorized for: Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ok status</returns>
        [AuthorizedFor(Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            try
            {
                await _taskRepository.DeleteTask(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
