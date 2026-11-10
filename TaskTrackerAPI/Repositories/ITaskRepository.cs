using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAllTasks();
        TaskItem? GetTaskById(int id);
        int? AddTask(CreateTaskModel task);
        bool UpdateTask(int id, CreateTaskModel updatedTask);
        bool DeleteTask(int id);
    }
}