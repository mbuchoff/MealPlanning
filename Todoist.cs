﻿using System.Net.Http;
using System.Net.Http.Json;
using SystemOfEquations.Data;

namespace SystemOfEquations;

internal class Todoist : IDisposable
{
    public async Task PushAsync(Phase phase)
    {
        Console.Write("Getting projects...");
        var project = await GetOrCreateProjectAsync("Automation");
        Console.WriteLine(" Done");

        Console.Write("Getting preexiting tasks from project...");
        var todoistTasks = await GetTasksFromProjectAsync(project.Id);
        Console.WriteLine(" Done");

        foreach (var todoistTask in todoistTasks)
        {
            if (todoistTask.Parent_Id == null)
            {
                Console.Write($"Deleting {todoistTask.Content}...");
                await DeleteTaskAsync(todoistTask.Id);
                Console.WriteLine(" Done");
            }
        }

        foreach (var x in new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals.Select(meal => new
        {
            trainingDay.TrainingDayType,
            Meal = meal,
        })))
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

                Console.Write($"Adding {content}...");
                var parentTodoistTask = await AddTaskAsync(content, dueString, parentId: null, project.Id);
                Console.WriteLine(" Done");

                foreach (var helping in x.Meal.Helpings)
                {
                    if (!helping.Food.IsConversion)
                    {
                        Console.Write($"Adding {helping} to {content}...");
                        await AddTaskAsync(
                            helping.ToString(), dueString: null, parentTodoistTask.Id, projectId: null);
                        Console.WriteLine(" Done");
                    }
                }
            }
        }
    }

    private async Task<TodoistTask> AddTaskAsync(
        string content, string? dueString, string? parentId, string? projectId)
    {
        var httpClient = await _httpClientTask;
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
        var httpClient = await _httpClientTask;
        var deleteResult = await httpClient.DeleteAsync($"https://api.todoist.com/rest/v2/tasks/{id}");
        deleteResult.EnsureSuccessStatusCode();
    }

    private async Task<TodoistTask[]> GetTasksFromProjectAsync(string id)
    {
        var httpClient = await _httpClientTask;
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
            var httpClient = await _httpClientTask;
            var projectsResponse = await httpClient.PostAsJsonAsync(
                "https://api.todoist.com/rest/v2/projects", new { Name = projectName });
            project = await projectsResponse.Content.ReadFromJsonAsync<Project>();
        }

        return project ?? throw new NullReferenceException(nameof(project));
    }

    private async Task<Project[]> GetProjectsAsync()
    {
        var httpClient = await _httpClientTask;
        var projectsResponse = await httpClient.GetAsync("https://api.todoist.com/rest/v2/projects");
        projectsResponse.EnsureSuccessStatusCode();
        var projects = await projectsResponse.Content.ReadFromJsonAsync<Project[]>();
        if (projects == null)
        {
            throw new NullReferenceException(nameof(projects));
        }
        return projects;
    }

    public void Dispose()
    {
        _httpClientTask.ContinueWith(task => task.Result.Dispose());
    }

    private readonly Task<HttpClient> _httpClientTask =
        File.ReadAllTextAsync(@"C:\Users\mbuch\OneDrive\Desktop\secrets\todoist.txt").ContinueWith(readAllTextTask =>
        {
            var apiKey = readAllTextTask.Result;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            return httpClient;
        });

    private record Project(string Id, string Name);
    private record TodoistTask(string Content, string Id, string? Parent_Id);
}
