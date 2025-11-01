using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace SystemOfEquations.Todoist;

internal static class TodoistApi
{
    /// <summary>
    /// Executes a batch of Sync API v9 commands and returns the response with temp ID mappings.
    /// Throws InvalidOperationException if any commands in the batch failed.
    /// </summary>
    public static async Task<SyncApiResponse> ExecuteBatchAsync(CommandBatch batch)
    {
        if (batch.IsEmpty)
        {
            return new SyncApiResponse(new Dictionary<string, System.Text.Json.JsonElement>(), new Dictionary<string, string>());
        }

        using var httpClient = await CreateHttpClientAsync();

        // Configure JSON options to ignore null values
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var requestBody = new { commands = batch.Commands };
        var result = await httpClient.PostAsJsonAsync("https://api.todoist.com/sync/v9/sync", requestBody, jsonOptions);
        result.EnsureSuccessStatusCode();

        var response = await result.Content.ReadFromJsonAsync<SyncApiResponse>();
        if (response == null)
            throw new NullReferenceException(nameof(response));

        // Check for any failed commands and throw with details
        try
        {
            response.ThrowIfAnyFailed();
        }
        catch (InvalidOperationException)
        {
            // Log the failing batch for debugging
            var json = System.Text.Json.JsonSerializer.Serialize(requestBody, jsonOptions);
            Console.Error.WriteLine($"\nFailing batch JSON (first 2000 chars):\n{json.Substring(0, Math.Min(2000, json.Length))}\n");
            throw;
        }

        return response;
    }

    public static async Task AddCommentAsync(string taskId, string comment)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync(
            "https://api.todoist.com/rest/v2/comments", new { Task_id = taskId, Content = comment });
        result.EnsureSuccessStatusCode();
    }

    public static async Task UpdateTaskCollapsedAsync(string taskId, bool collapsed)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync("https://api.todoist.com/sync/v9/sync", new
        {
            Commands = new[]
            {
                new
                {
                    Type = "item_update",
                    Uuid = Guid.NewGuid().ToString(),
                    Args = new
                    {
                        Id = taskId,
                        Collapsed = collapsed
                    }
                }
            }
        });
        result.EnsureSuccessStatusCode();
    }

    public static async Task<TodoistTask> AddTaskAsync(
        string content, string? description, string? dueString, string? parentId, string? projectId, bool isCollapsed = false, int? order = null)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync("https://api.todoist.com/rest/v2/tasks", new
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
        var deleteResult = await httpClient.DeleteAsync($"https://api.todoist.com/rest/v2/tasks/{id}");
        deleteResult.EnsureSuccessStatusCode();
    }

    public static async Task<TodoistTask[]> GetTasksFromProjectAsync(string id)
    {
        using var httpClient = await CreateHttpClientAsync();
        var tasksResponse = await httpClient.GetAsync($"https://api.todoist.com/rest/v2/tasks?project_id={id}");
        var todoistTasks = await tasksResponse.Content.ReadFromJsonAsync<TodoistTask[]>();
        return todoistTasks ?? throw new NullReferenceException(nameof(TodoistTask));
    }

    public static async Task<Project[]> GetProjectsAsync()
    {
        using var httpClient = await CreateHttpClientAsync();
        var projectsResponse = await httpClient.GetAsync("https://api.todoist.com/rest/v2/projects");
        projectsResponse.EnsureSuccessStatusCode();
        var projects = await projectsResponse.Content.ReadFromJsonAsync<Project[]>();
        return projects ?? throw new NullReferenceException(nameof(projects));
    }

    public static async Task<Project> AddProjectAsync(string projectName)
    {
        using var httpClient = await CreateHttpClientAsync();
        var projectsResponse = await httpClient.PostAsJsonAsync(
            "https://api.todoist.com/rest/v2/projects", new { Name = projectName });
        var project = await projectsResponse.Content.ReadFromJsonAsync<Project>();
        return project ?? throw new Exception(nameof(project));
    }

    private static async Task<HttpClient> CreateHttpClientAsync()
    {
        var apiKey = await GetApiKeyAsync();
        var httpClient = new HttpClient(new HttpClientRetryHandler());
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        return httpClient;
    }

    private static string? _apiKey;
    private static Task<string> GetApiKeyAsync()
    {
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

    public record Project(string Id, string Name);
    public record TodoistTask(string Content, DateTimeOffset Created_at, string Id, string? Parent_Id);
}
