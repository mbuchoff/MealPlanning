using System.Net.Http.Json;

namespace SystemOfEquations.Todoist;

internal class TodoistApi
{
    public static async Task<TodoistTask> AddTaskAsync(
        string content, string? dueString, string? parentId, string? projectId)
    {
        using var httpClient = await CreateHttpClientAsync();
        var result = await httpClient.PostAsJsonAsync("https://api.todoist.com/rest/v2/tasks", new
        {
            Content = content,
            Parent_id = parentId,
            Project_id = projectId,
            Due_string = dueString,
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
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        return httpClient;
    }

    private static string? _apiKey;
    private static async Task<string> GetApiKeyAsync()
    {
        _apiKey ??= await File.ReadAllTextAsync(@"C:\Users\mbuch\OneDrive\Desktop\secrets\todoist.txt");
        return _apiKey;
    }

    public record Project(string Id, string Name);
    public record TodoistTask(string Content, string Id, string? Parent_Id);
}
