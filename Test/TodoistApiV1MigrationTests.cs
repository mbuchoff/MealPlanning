using System.Net;
using System.Text;
using System.Text.Json;
using SystemOfEquations.Todoist;

namespace SystemOfEquations.Test;

public class TodoistApiV1MigrationTests : IDisposable
{
    public void Dispose()
    {
        TodoistApi.ResetHttpOverridesForTests();
    }

    [Fact]
    public async Task GetProjectsAsync_Should_Use_ApiV1_And_Parse_Wrapped_Results()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("https://api.todoist.com/api/v1/projects", request.RequestUri?.ToString());

            var body = JsonSerializer.Serialize(new
            {
                results = new[]
                {
                    new { id = "project_1", name = "Eating" }
                }
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
        });

        TodoistApi.SetHttpOverridesForTests("test-api-key", handler);

        var projects = await TodoistApi.GetProjectsAsync();

        Assert.Single(projects);
        Assert.Equal("project_1", projects[0].Id);
        Assert.Equal("Eating", projects[0].Name);
    }

    [Fact]
    public async Task GetTasksFromProjectAsync_Should_Use_ApiV1_And_Map_AddedAt_To_CreatedAt()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal(
                "https://api.todoist.com/api/v1/tasks?project_id=project_123",
                request.RequestUri?.ToString());

            var body = JsonSerializer.Serialize(new
            {
                results = new[]
                {
                    new
                    {
                        content = "Task 1",
                        added_at = "2026-02-11T20:00:00Z",
                        id = "task_1",
                        parent_id = "parent_1"
                    }
                }
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
        });

        TodoistApi.SetHttpOverridesForTests("test-api-key", handler);

        var tasks = await TodoistApi.GetTasksFromProjectAsync("project_123");

        Assert.Single(tasks);
        Assert.Equal("task_1", tasks[0].Id);
        Assert.Equal("Task 1", tasks[0].Content);
        Assert.Equal("parent_1", tasks[0].Parent_Id);
        Assert.Equal(DateTimeOffset.Parse("2026-02-11T20:00:00Z"), tasks[0].Created_at);
    }

    private class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(handler(request));
        }
    }
}
