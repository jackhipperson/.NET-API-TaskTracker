using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _context;
        public TaskRepository(TaskDbContext context)
        {
            _context = context;
        }
        public IEnumerable<TaskItem> GetAllTasks()
        {
            return _context.Tasks.ToList();
        }
        public TaskItem? GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }
        public TaskItem AddTask(CreateTaskModel task)
        {
            var newTask = new TaskItem
            {
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(newTask);
            _context.SaveChanges();
            return newTask;

        }
        public bool UpdateTask(int id, CreateTaskModel updatedTask)
        {

            var existingTask = GetTaskById(id);

            if (existingTask is null)
            {
                return false;
            }
            else
            {
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.IsCompleted = updatedTask.IsCompleted;

                _context.SaveChanges();
                return true;
            }


        }
        public bool DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);

            if (task is null)
            {
                return false;
            }
            else
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
                return true;
            }


        }

    }
}
