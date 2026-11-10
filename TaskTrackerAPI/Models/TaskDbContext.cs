using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Entities;

namespace TaskTrackerAPI.Models
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions options) : base(options)
        {

            
        }

        protected TaskDbContext()
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }

    }
}
