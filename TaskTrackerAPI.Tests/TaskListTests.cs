using System.Net;
using System.Net.Http.Json;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Tests.ApiFactory;

namespace TaskTrackerAPI.Tests
{
    public class TaskListTests : IClassFixture<TaskApiFactory>
    {
        private readonly TaskApiFactory _factory;
        private readonly HttpClient _client;
        private readonly DateTime _createdAt;

        private TaskItem[] SeedItems() => new [] 
        {
            new TaskItem { Id = 1,Title = "Test Task 1",Description = "This is a test task.",IsCompleted = false,CreatedAt = _createdAt },
            new TaskItem { Id = 2,Title = "Test Task 2",Description = "This is a second test task.",IsCompleted = false,CreatedAt = _createdAt },
            new TaskItem { Id = 3,Title = "Test Task 3",Description = "This is a thirdtest task.",IsCompleted = true,CreatedAt = _createdAt },
        };

        public TaskListTests(TaskApiFactory factory)
        {
            _factory = factory;
            _factory.ResetDatabase();
            _createdAt = DateTime.UtcNow;
            _client = _factory.CreateClient();
            _factory.Seed(SeedItems());
        }


        [Fact]
        public async Task SuccessfullyRetrieveTasks()
        {

            var result = await _client.GetAsync("/api/tasks");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var returned = await result.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();

            //check ids are in the table
            Assert.Equal(new[] {1,2,3}, returned?.Select(t => t.Id).OrderBy(id => id));
        }



        [Fact]
        public async Task SuccessfullyRetrieveTask()
        {

            var result = await _client.GetAsync("/api/tasks/2");
            var returned = await result.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            //check ids are in the table
            Assert.Equal(2, returned?.Id);
            Assert.Equal("Test Task 2", returned?.Title);
            Assert.Equal("This is a second test task.", returned?.Description);
            Assert.False(returned?.IsCompleted);
        }


        [Fact]
        public async Task SuccessfullyCreateTasks()
        {

            var newTaskItem = new CreateTaskModel
            {
                Title = "Test Task 4",
                Description = "This is a fourth test task.",
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            var verifyResult = await _client.GetAsync(result.Headers.Location);
            var returned = await verifyResult.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal(HttpStatusCode.OK, verifyResult.StatusCode);
            Assert.Equal(newTaskItem.Title, returned?.Title);
            Assert.Equal(newTaskItem.Description, returned?.Description);
            Assert.Equal(newTaskItem.IsCompleted, returned?.IsCompleted);
        }

        [Fact]
        public async Task SuccessfullyUpdateTasks()
        {
            
            var amendedTask = new CreateTaskModel
            {
                Title = "Test Task 4",
                Description = "This is a fourth test task.",
                IsCompleted = false,
            };

            var result = await _client.PutAsJsonAsync("/api/tasks/1", amendedTask);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks/1");
            var returned = await verifyResult.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal(HttpStatusCode.OK, verifyResult.StatusCode);
            Assert.Equal(amendedTask.Title, returned?.Title);
            Assert.Equal(amendedTask.Description, returned?.Description);
            Assert.Equal(amendedTask.IsCompleted, returned?.IsCompleted);
            Assert.Equal(_createdAt, returned?.CreatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UnsuccessfullyUpdateTasks_Title(string? title)
        {
          
            var amendedTask = new CreateTaskModel
            {
                Title = title,
                Description = "This is a fourth test task.",
                IsCompleted = true,
            };

            var result = await _client.PutAsJsonAsync("/api/tasks/1", amendedTask);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks/1");
            var returned = await verifyResult.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal("This is a test task.", returned?.Description);
            Assert.False(returned?.IsCompleted);

        }

        [Fact]
        public async Task UnsuccessfullyUpdateTasks_TitleLength()
        {
           
            var amendedTask = new CreateTaskModel
            {
                Title = new string('A', 101),
                Description = "This is an amended test task.",
                IsCompleted = true,
            };

            var result = await _client.PutAsJsonAsync("/api/tasks/1", amendedTask);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks/1");
            var returned = await verifyResult.Content.ReadFromJsonAsync<TaskItem>();

            Assert.NotEqual("This is an amended test task.", returned?.Description);
            Assert.False(returned?.IsCompleted);

        }
        [Fact]
        public async Task UnsuccessfullyUpdateTasks_DescriptionLength()
        {
            var amendedTask = new CreateTaskModel
            {
                Title = "This is the new title",
                Description = new string('A', 501),
                IsCompleted = true,
            };

            var result = await _client.PutAsJsonAsync("/api/tasks/1", amendedTask);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verify = await _client.GetAsync("/api/tasks/1");
            var returned = await verify.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal("Test Task 1", returned?.Title);
            Assert.False(returned?.IsCompleted);

        }

        [Fact]
        public async Task UnsuccessfullyUpdateTasks_Id()
        {
            var amendedTask = new CreateTaskModel
            {
                Title = "Amended Task",
                Description = "This is another test task.",
                IsCompleted = true,
            };

            var result = await _client.PutAsJsonAsync("/api/tasks/100", amendedTask);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks/1");
            var returned = await verifyResult.Content.ReadFromJsonAsync<TaskItem>();

            Assert.Equal("Test Task 1", returned?.Title);
            Assert.Equal("This is a test task.", returned?.Description);
            Assert.False(returned?.IsCompleted);

        }

        [Fact]
        public async Task SuccessfullyDeleteTasks()
        {
            var result = await _client.DeleteAsync("/api/tasks/1");
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks/1");
            Assert.Equal(HttpStatusCode.NotFound, verifyResult.StatusCode);

        }
        [Fact]
        public async Task UnsuccessfullyDeleteTasks()
        {
            var result = await _client.DeleteAsync("/api/tasks/100");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks");
            var returned = await verifyResult.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();

            //check ids are in the table
            Assert.Equal(new[] { 1, 2, 3 }, returned?.Select(t => t.Id).OrderBy(id => id));

        }



        [Fact]
        public async Task Get_TaskDoesntExist()
        {

            var result = await _client.GetAsync("/api/tasks/100");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

        }
       

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task TitleError(string? title)
        {

            var newTaskItem = new CreateTaskModel
            {
                Title = title,
                Description = "This is a test task.",
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verifyResult = await _client.GetAsync("/api/tasks");
            var returned = await verifyResult.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();

            //check ids are in the table
            Assert.Equal(new[] { 1, 2, 3 }, returned?.Select(t => t.Id).OrderBy(id => id));

        }

        [Fact]
        public async Task TitleExactly100Characters()
        {

            var newTaskItem = new CreateTaskModel
            {
                Title = new string('A', 100),
                Description = "This is a fourth test task.",
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            var verify = await _client.GetAsync(result.Headers.Location);
            var returned = await verify.Content.ReadFromJsonAsync<TaskItem>();

            Assert.NotNull(returned);
            Assert.Equal(newTaskItem.Title, returned.Title);
            Assert.Equal(newTaskItem.Description, returned.Description);
            Assert.Equal(newTaskItem.IsCompleted, returned.IsCompleted);

        }

        [Fact]
        public async Task TitleExceeds100Characters()
        {

            var original = await _client.GetAsync("/api/tasks");
            var originalList = await original.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();
            var originalCount = originalList?.Count() ?? 0;

            var newTaskItem = new CreateTaskModel
            {
                Title = new string('A', 101),
                Description = "This is a fourth test task.",
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verify = await _client.GetAsync("/api/tasks");
            var verifyList = await verify.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();
            var verifyCount = verifyList?.Count() ?? 0;

            Assert.Equal(originalCount, verifyCount);
        }

        [Fact]
        public async Task DescriptionExactly500Characters()
        {

            var newTaskItem = new CreateTaskModel
            {
                Title = "Test Title",
                Description = new string('A', 500),
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            var verify = await _client.GetAsync(result.Headers.Location);
            var returned = await verify.Content.ReadFromJsonAsync<TaskItem>();

            Assert.NotNull(returned);
            Assert.Equal(newTaskItem.Title, returned.Title);
            Assert.Equal(newTaskItem.Description, returned.Description);
            Assert.Equal(newTaskItem.IsCompleted, returned.IsCompleted);
        }

        [Fact]
        public async Task DescriptionExceeds500Characters()
        {

            var original = await _client.GetAsync("/api/tasks");
            var originalList = await original.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();
            var originalCount = originalList?.Count() ?? 0;

            var newTaskItem = new CreateTaskModel
            {
                Title = "Test Title",
                Description = new string('A', 501),
                IsCompleted = false,
            };

            var result = await _client.PostAsJsonAsync("/api/tasks", newTaskItem);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var verify = await _client.GetAsync("/api/tasks");
            var verifyList = await verify.Content.ReadFromJsonAsync<IEnumerable<TaskItem>>();
            var verifyCount = verifyList?.Count() ?? 0;

            Assert.Equal(originalCount, verifyCount);
        }




    }
}
