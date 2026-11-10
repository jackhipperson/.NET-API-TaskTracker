using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Tests.ApiFactory
{
    public class TaskApiFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:");
        public TaskApiFactory()
        {
            _connection.Open();
            using var scope = Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<TaskDbContext>().Database.EnsureCreated();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<TaskDbContext>>();
                services.AddDbContext<TaskDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });
        }

        public void ResetDatabase()
        {
            using var scope = Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<TaskDbContext>().Tasks.ExecuteDelete();
        }

        public void Seed(params TaskItem[] items)
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            context.Tasks.AddRange(items);
            context.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
