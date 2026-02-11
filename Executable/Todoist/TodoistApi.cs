using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace SystemOfEquations.Todoist;

internal static class TodoistApi
{
    private const string ApiV1BaseUrl = "https://api.todoist.com/api/v1";

    public static async Task AddCommentAsync(string taskId, string comment)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync(
            $"{ApiV1BaseUrl}/comments", new { Task_id = taskId, Content = comment });
        result.EnsureSuccessStatusCode();
    }

    public static Task UpdateTaskCollapsedAsync(string taskId, bool collapsed)
    {
        return Task.CompletedTask;
    }

    public static async Task<TodoistTask> AddTaskAsync(
        string content, string? description, string? dueString, string? parentId, string? projectId, bool isCollapsed = false, int? order = null)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync($"{ApiV1BaseUrl}/tasks", new
        {
            Content = content,
            Description = description,
            Parent_id = parentId,
            Project_id = projectId,
            Due_string = dueString,
            Is_collapsed = isCollapsed,
            Order = order,
        });
        result.EnsureSuccessStatusCode();
        var todoistTask = await result.Content.ReadFromJsonAsync<TodoistTask>();
        return todoistTask ?? throw new NullReferenceException(nameof(todoistTask));
    }

    public static async Task DeleteTaskAsync(string id)
    {
        using var httpClient = await CreateHttpClientAsync();
        var deleteResult = await httpClient.DeleteAsync($"{ApiV1BaseUrl}/tasks/{id}");
        deleteResult.EnsureSuccessStatusCode();
    }

    public static async Task<TodoistTask[]> GetTasksFromProjectAsync(string id)
    {
        using var httpClient = await CreateHttpClientAsync();
        var tasksResponse = await httpClient.GetAsync($"{ApiV1BaseUrl}/tasks?project_id={id}");
        tasksResponse.EnsureSuccessStatusCode();
        var todoistTasks = await ReadResultsAsync<TodoistTask>(tasksResponse);
        return todoistTasks ?? throw new NullReferenceException(nameof(TodoistTask));
    }

    public static async Task<Project[]> GetProjectsAsync()
    {
        using var httpClient = await CreateHttpClientAsync();
        var projectsResponse = await httpClient.GetAsync($"{ApiV1BaseUrl}/projects");
        projectsResponse.EnsureSuccessStatusCode();
        var projects = await ReadResultsAsync<Project>(projectsResponse);
        return projects ?? throw new NullReferenceException(nameof(projects));
    }

    public static async Task<Project> AddProjectAsync(string projectName)
    {
        using var httpClient = await CreateHttpClientAsync();
        var projectsResponse = await httpClient.PostAsJsonAsync(
            $"{ApiV1BaseUrl}/projects", new { Name = projectName });
        projectsResponse.EnsureSuccessStatusCode();
        var project = await projectsResponse.Content.ReadFromJsonAsync<Project>();
        return project ?? throw new Exception(nameof(project));
    }

    private static async Task<T[]> ReadResultsAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<TodoistListResponse<T>>();
        return result?.Results ?? throw new NullReferenceException(nameof(result.Results));
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
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
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

    private record TodoistListResponse<T>(T[] Results);

    public record Project(string Id, string Name);
    public record TodoistTask(
        string Content,
        [property: JsonPropertyName("added_at")] DateTimeOffset Created_at,
        string Id,
        [property: JsonPropertyName("parent_id")] string? Parent_Id);
}
