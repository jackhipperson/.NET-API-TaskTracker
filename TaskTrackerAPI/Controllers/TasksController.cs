using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Repositories;

namespace TaskTrackerAPI.Controllers
{
    [ApiController]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("/api/tasks")]
        [ProducesResponseType(typeof(List<TaskItem>), StatusCodes.Status200OK)]
        public IActionResult Index()
        {
            return Ok(_taskRepository.GetAllTasks());
        }

        [HttpGet("/api/tasks/{id}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTask(int id)
        {
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("/api/tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTask([FromBody] CreateTaskModel task)
        {

            var id = _taskRepository.AddTask(task);
            if (id == null)
                return BadRequest();

            var created = _taskRepository.GetTaskById(id.Value);

            return CreatedAtAction(nameof(GetTask), new { id = id.Value }, created);
        }

        [HttpPut("/api/tasks/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateTask(int id, [FromBody] CreateTaskModel task)
        {
            if (_taskRepository.GetTaskById(id) == null)
                return NotFound();

            return _taskRepository.UpdateTask(id, task) ? new NoContentResult() : new BadRequestResult();
        }

        [HttpDelete("/api/tasks/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteTask(int id)
        {
            if (_taskRepository.GetTaskById(id) == null)
                return NotFound();

            return _taskRepository.DeleteTask(id) ? new NoContentResult() : new BadRequestResult();
        }
    }
}
