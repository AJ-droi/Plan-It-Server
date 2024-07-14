using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plan_It.Dto.Task;
using Plan_It.Interfaces;
using Plan_It.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Plan_It.Dto.Query;

namespace Plan_It.Controllers
{
    [Route("task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITask _taskRepo;
        private readonly ILogger<TaskController> _logger;

        public TaskController(UserManager<ApplicationUser> userManager, ITask taskRepo, ILogger<TaskController> logger)
        {
            _userManager = userManager;
            _taskRepo = taskRepo;
            _logger = logger;
        }

        [HttpPost("create-task")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation($"{email}");
                if (email == null)
                {
                    _logger.LogWarning("Unauthorized attempt to create task. Email claim not found.");
                    return Unauthorized();
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    _logger.LogWarning("Unauthorized attempt to create task. User not found.");
                    return Unauthorized();
                }


                var newTask = new Models.Task
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Category = createTaskDto.Category,
                    Priority = createTaskDto.Priority,
                    StartDate = createTaskDto.StartDate.ToUniversalTime(),
                    StartTime = createTaskDto.StartTime,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var createdTask = await _taskRepo.CreateAsync(newTask);

                _logger.LogInformation($"Task created successfully for user {user.UserName}");

                return Ok(createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetSingleTask([FromRoute] Guid id)
        {
            try
            {
                var task = await _taskRepo.TaskInfo(id);

                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get-all-tasks")]
        [Authorize]
        public async Task<IActionResult> GetAllTasks([FromQuery] TaskDto query)
        {
            try
            {
                var tasks = await _taskRepo.GetAllTasks(query);

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var task = await _taskRepo.TaskInfo(id);

                if (task == null)
                {
                    return NotFound();
                }

                task.Title = updateTaskDto.Title;
                task.Description = updateTaskDto.Description;
                task.Category = updateTaskDto.Category;
                task.Priority = updateTaskDto.Priority;
                task.StartDate = updateTaskDto.StartDate.ToUniversalTime();
                task.DueDate = updateTaskDto.DueDate.ToUniversalTime();
                task.TaskStatus = updateTaskDto.TaskStatus;
                task.StartTime = updateTaskDto.StartTime;
                task.UpdatedAt = DateTime.UtcNow;

                var updatedTask = await _taskRepo.UpdateTask(id, task);

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            try
            {

                var task = await _taskRepo.RemoveTask(id);

                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("assign-group/{id:Guid}")]
        [Authorize]

        public async Task<IActionResult> AssignGroupToTask([FromRoute] Guid id, Guid groupId)
        {

            try
            {
                var taskGroup = await _taskRepo.AssignGroupToTask(id, groupId);

                if (taskGroup == null)
                {
                    return NotFound();
                }

                if (taskGroup is string)
                {
                    return BadRequest(taskGroup); // Return the error message
                }

                return Ok(taskGroup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
