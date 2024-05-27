using System.Net.Http.Json;
using SystemOfEquations.Data;

namespace SystemOfEquations;

internal class Todoist
{
    public async Task PushAsync(Phase phase)
    {
        var project = await GetOrCreateProjectAsync("Automation");

        var deleteTask = new Task(async () =>
        {
            var todoistTasks = await GetTasksFromProjectAsync(project.Id);

            await Task.WhenAll(todoistTasks
                .Where(todoistTask => todoistTask.Parent_Id == null)
                .Select(todoistTask => DeleteTaskAsync(todoistTask.Id)).ToList());
        });
        deleteTask.Start();

        var addTasks = new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals.Select(meal => new
        {
            trainingDay.TrainingDayType,
            Meal = meal,
        })).Select(async x =>
        {
            if (x.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
            {
                var content = $"{x.TrainingDayType} - {x.Meal.Name}";
                var dueString = new Dictionary<TrainingDayType, string>()
                {
                    { TrainingDayTypes.XfitDay, "every mon,wed,fri" },
                    { TrainingDayTypes.RunningDay, "every tue,thu" },
                    { TrainingDayTypes.NonweightTrainingDay, "every sat,sun" },
                }.GetValueOrDefault(x.TrainingDayType);

                var parentTodoistTask = await AddTaskAsync(
                    $"{content} (added {DateTime.Now})", dueString, parentId: null, project.Id);

                await Task.WhenAll(x.Meal.Helpings.Where(h => !h.Food.IsConversion).Select(h => AddTaskAsync(
                        h.ToString(), dueString: null, parentTodoistTask.Id, projectId: null)));
            }
        }).ToList();

        await deleteTask;
        await Task.WhenAll(addTasks);
    }

    private async Task<TodoistTask> AddTaskAsync(
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

    private async Task DeleteTaskAsync(string id)
    {
        using var httpClient = await CreateHttpClientAsync();
        var deleteResult = await httpClient.DeleteAsync($"https://api.todoist.com/rest/v2/tasks/{id}");
        deleteResult.EnsureSuccessStatusCode();
    }

    private async Task<TodoistTask[]> GetTasksFromProjectAsync(string id)
    {
        using var httpClient = await CreateHttpClientAsync();
        var tasksResponse = await httpClient.GetAsync($"https://api.todoist.com/rest/v2/tasks?project_id={id}");
        var todoistTasks = await tasksResponse.Content.ReadFromJsonAsync<TodoistTask[]>();
        return todoistTasks ?? throw new NullReferenceException(nameof(TodoistTask));
    }

    private async Task<Project> GetOrCreateProjectAsync(string projectName)
    {
        var projects = await GetProjectsAsync();
        var project = projects.SingleOrDefault(p => p.Name == projectName);

        if (project == null)
        {
            using var httpClient = await CreateHttpClientAsync();
            var projectsResponse = await httpClient.PostAsJsonAsync(
                "https://api.todoist.com/rest/v2/projects", new { Name = projectName });
            project = await projectsResponse.Content.ReadFromJsonAsync<Project>();
        }

        return project ?? throw new NullReferenceException(nameof(project));
    }

    private async Task<Project[]> GetProjectsAsync()
    {
        using var httpClient = await CreateHttpClientAsync();
        var projectsResponse = await httpClient.GetAsync("https://api.todoist.com/rest/v2/projects");
        projectsResponse.EnsureSuccessStatusCode();
        var projects = await projectsResponse.Content.ReadFromJsonAsync<Project[]>();
        if (projects == null)
        {
            throw new NullReferenceException(nameof(projects));
        }
        return projects;
    }

    private string? _apiKey;
    private async Task<string> GetApiKeyAsync()
    {
        _apiKey ??= await File.ReadAllTextAsync(@"C:\Users\mbuch\OneDrive\Desktop\secrets\todoist.txt");
        return _apiKey;
    }

    private async Task<HttpClient> CreateHttpClientAsync()
    {
        var apiKey = await GetApiKeyAsync();
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        return httpClient;
    }
    private record Project(string Id, string Name);
    private record TodoistTask(string Content, string Id, string? Parent_Id);
}
