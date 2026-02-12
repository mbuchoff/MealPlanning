using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace SystemOfEquations.Todoist;

internal static class TodoistApi
{
    private const string ApiV1BaseUrl = "https://api.todoist.com/api/v1";
    private static readonly SemaphoreSlim TodoistConcurrencyGate = new(initialCount: 4, maxCount: 4);

    public static async Task AddCommentAsync(string taskId, string comment)
    {
        await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var result = await httpClient.PostAsJsonAsync(
                $"{ApiV1BaseUrl}/comments",
                new AddCommentRequest(taskId, comment));
            await EnsureSuccessWithDetailsAsync(result);
        });
    }

    public static Task UpdateTaskCollapsedAsync(string taskId, bool collapsed)
    {
        return Task.CompletedTask;
    }

    public static async Task<TodoistTask> AddTaskAsync(
        string content, string? description, string? dueString, string? parentId, string? projectId, bool isCollapsed = false, int? order = null)
    {
        return await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var result = await httpClient.PostAsJsonAsync(
                $"{ApiV1BaseUrl}/tasks",
                new AddTaskRequest(content, description, dueString, parentId, projectId, isCollapsed, order));
            await EnsureSuccessWithDetailsAsync(result);
            var todoistTask = await result.Content.ReadFromJsonAsync<TodoistTask>();
            return todoistTask ?? throw new NullReferenceException(nameof(todoistTask));
        });
    }

    public static async Task DeleteTaskAsync(string id)
    {
        await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var deleteResult = await httpClient.DeleteAsync($"{ApiV1BaseUrl}/tasks/{id}");
            await EnsureSuccessWithDetailsAsync(deleteResult);
        });
    }

    public static async Task<TodoistTask[]> GetTasksFromProjectAsync(string id)
    {
        return await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var tasksResponse = await httpClient.GetAsync($"{ApiV1BaseUrl}/tasks?project_id={id}");
            await EnsureSuccessWithDetailsAsync(tasksResponse);
            var todoistTasks = await ReadResultsAsync<TodoistTask>(tasksResponse);
            return todoistTasks ?? throw new NullReferenceException(nameof(todoistTasks));
        });
    }

    public static async Task<Project[]> GetProjectsAsync()
    {
        return await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var projectsResponse = await httpClient.GetAsync($"{ApiV1BaseUrl}/projects");
            await EnsureSuccessWithDetailsAsync(projectsResponse);
            var projects = await ReadResultsAsync<Project>(projectsResponse);
            return projects ?? throw new NullReferenceException(nameof(projects));
        });
    }

    public static async Task<Project> AddProjectAsync(string projectName)
    {
        return await WithTodoistConcurrencyAsync(async () =>
        {
            using var httpClient = await CreateHttpClientAsync();
            var projectsResponse = await httpClient.PostAsJsonAsync(
                $"{ApiV1BaseUrl}/projects",
                new AddProjectRequest(projectName));
            await EnsureSuccessWithDetailsAsync(projectsResponse);
            var project = await projectsResponse.Content.ReadFromJsonAsync<Project>();
            return project ?? throw new Exception(nameof(project));
        });
    }

    private static async Task WithTodoistConcurrencyAsync(Func<Task> action)
    {
        await TodoistConcurrencyGate.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            TodoistConcurrencyGate.Release();
        }
    }

    private static async Task<T> WithTodoistConcurrencyAsync<T>(Func<Task<T>> action)
    {
        await TodoistConcurrencyGate.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            TodoistConcurrencyGate.Release();
        }
    }

    private static async Task<T[]> ReadResultsAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<TodoistListResponse<T>>();
        return result?.Results ?? throw new NullReferenceException(nameof(result.Results));
    }

    private static async Task EnsureSuccessWithDetailsAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        string? body = null;
        try
        {
            body = await response.Content.ReadAsStringAsync();
        }
        catch
        {
            // ignore - we still want a useful exception even if the body can't be read
        }

        var request = response.RequestMessage;
        var requestInfo = request == null
            ? "<unknown request>"
            : $"{request.Method} {request.RequestUri}";

        var details = string.IsNullOrWhiteSpace(body) ? "" : $" Body: {body}";

        throw new HttpRequestException(
            $"Todoist API request failed: {requestInfo}. Status: {(int)response.StatusCode} ({response.StatusCode}).{details}");
    }

    internal static void SetHttpOverridesForTests(string apiKey, HttpMessageHandler handler)
    {
        _apiKeyOverride = apiKey;
        _httpMessageHandlerOverride = handler;
    }

    internal static void ResetHttpOverridesForTests()
    {
        _apiKeyOverride = null;
        _httpMessageHandlerOverride = null;
    }

    private static async Task<HttpClient> CreateHttpClientAsync()
    {
        var apiKey = await GetApiKeyAsync();
        var httpClient = _httpMessageHandlerOverride == null
            ? new HttpClient(new HttpClientRetryHandler(
                new HttpClientHandler(),
                message => ProgressTracker.Current?.UpdateMessage(message)))
            : new HttpClient(_httpMessageHandlerOverride, disposeHandler: false);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        return httpClient;
    }

    private static string? _apiKey;
    private static string? _apiKeyOverride;
    private static HttpMessageHandler? _httpMessageHandlerOverride;

    private static Task<string> GetApiKeyAsync()
    {
        if (!string.IsNullOrEmpty(_apiKeyOverride))
            return Task.FromResult(_apiKeyOverride);

        if (_apiKey == null)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            _apiKey = configuration["Todoist:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException(
                    "Todoist API key not found. Please run: dotnet user-secrets set \"Todoist:ApiKey\" \"YOUR_API_KEY\"");
            }
        }
        return Task.FromResult(_apiKey);
    }

    private record AddCommentRequest(
        [property: JsonPropertyName("task_id")] string TaskId,
        [property: JsonPropertyName("content")] string Content);

    private record AddProjectRequest(
        [property: JsonPropertyName("name")] string Name);

    private record AddTaskRequest(
        [property: JsonPropertyName("content")] string Content,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("due_string")] string? DueString,
        [property: JsonPropertyName("parent_id")] string? ParentId,
        [property: JsonPropertyName("project_id")] string? ProjectId,
        [property: JsonPropertyName("is_collapsed")] bool IsCollapsed,
        [property: JsonPropertyName("order")] int? Order);

    private record TodoistListResponse<T>(T[] Results);

    public record Project(string Id, string Name);
    public record TodoistTask(
        string Content,
        [property: JsonPropertyName("added_at")] DateTimeOffset Created_at,
        string Id,
        [property: JsonPropertyName("parent_id")] string? Parent_Id);
}
