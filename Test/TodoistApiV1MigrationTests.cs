using System.Net;
using System.Text;
using System.Text.Json;
using SystemOfEquations.Todoist;

namespace SystemOfEquations.Test;

[Collection("TodoistApi")]
public class TodoistApiV1Tests : IDisposable
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
            Assert.Equal("Bearer test-api-key", request.Headers.Authorization?.ToString());

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
            Assert.Equal("Bearer test-api-key", request.Headers.Authorization?.ToString());

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

    [Fact]
    public async Task AddTaskAsync_Should_Use_ApiV1_And_Send_SnakeCase_Fields()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://api.todoist.com/api/v1/tasks", request.RequestUri?.ToString());
            Assert.Equal("Bearer test-api-key", request.Headers.Authorization?.ToString());

            var json = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.False(string.IsNullOrWhiteSpace(json));

            using var doc = JsonDocument.Parse(json!);
            var root = doc.RootElement;

            Assert.Equal("My Task", root.GetProperty("content").GetString());
            Assert.Equal("desc", root.GetProperty("description").GetString());
            Assert.Equal("every tue", root.GetProperty("due_string").GetString());
            Assert.Equal("parent_1", root.GetProperty("parent_id").GetString());
            Assert.Equal("project_1", root.GetProperty("project_id").GetString());
            Assert.True(root.GetProperty("is_collapsed").GetBoolean());
            Assert.Equal(7, root.GetProperty("order").GetInt32());

            var responseBody = JsonSerializer.Serialize(new
            {
                id = "task_123",
                content = "My Task",
                added_at = "2026-02-12T00:00:00Z",
                parent_id = "parent_1"
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        TodoistApi.SetHttpOverridesForTests("test-api-key", handler);

        var task = await TodoistApi.AddTaskAsync(
            "My Task",
            description: "desc",
            dueString: "every tue",
            parentId: "parent_1",
            projectId: "project_1",
            isCollapsed: true,
            order: 7);

        Assert.Equal("task_123", task.Id);
        Assert.Equal("My Task", task.Content);
        Assert.Equal("parent_1", task.Parent_Id);
        Assert.Equal(DateTimeOffset.Parse("2026-02-12T00:00:00Z"), task.Created_at);
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
