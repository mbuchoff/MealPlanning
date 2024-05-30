using static SystemOfEquations.Todoist.TodoistApi;

namespace SystemOfEquations.Todoist;

internal class TodoistService
{
    public static async Task SyncAsync(Phase phase)
    {
        var eatingProject = await GetOrCreateProjectAsync("Eating");
        var cookingProject = await GetOrCreateProjectAsync("Cooking");

        await Task.WhenAll(
            DeleteTasksFromProjectAsync(eatingProject.Id, createdBeforeUtc: DateTime.UtcNow),
            DeleteTasksFromProjectAsync(cookingProject.Id, createdBeforeUtc: DateTime.UtcNow),
            AddPhaseAsync(phase, eatingProject.Id, cookingProject.Id));
    }

    private static async Task AddPhaseAsync(Phase phase, string eatingProjectId, string cookingProjectId)
    {
        List<Task> systemTasks = phase.MealPrepPlan.Meals.Select(async m =>
        {
            var parentTask = await AddTaskAsync(m.Name, description: null, dueString: null, parentId: null, cookingProjectId);
            await Task.WhenAll(m.Helpings.Select(h => AddTaskAsync(
                h.ToString(), description: null, dueString: null, parentTask.Id, projectId: null)));
        }).ToList();

        foreach (var x in new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals.Select(meal => new
        {
            DueString = GetDueString(trainingDay.TrainingDayType),
            trainingDay.TrainingDayType,
            Meal = meal,
        })).Where(x => x.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
        {
            var content = $"{x.TrainingDayType} - {x.Meal.Name}";

            // Parent tasks need to be added in order so that they appear in order, so don't run them in parallel
            var parentTodoistTask = await AddTaskAsync(
                content, $"Synced on {DateTime.Now}",
                x.DueString, parentId: null, eatingProjectId);

            // Add child tasks in parallel
            systemTasks.AddRange(x.Meal.Helpings.Where(h => !h.Food.IsConversion)
                .Select(h => AddTaskAsync(
                    h.ToString(), description: null, dueString: null, parentTodoistTask.Id, projectId: null)));

            var comment = string.Join("\n\n",
                new[] { x.Meal.NutritionalInformation.ToNutrientsString() }.Concat(
                x.Meal.Helpings.Select(h => $"{h.Food.Name}\n{h.NutritionalInformation.ToNutrientsString()}")));
            systemTasks.Add(AddCommentAsync(parentTodoistTask.Id, comment));
        }

        await Task.WhenAll(systemTasks);
    }

    private static async Task<Project> GetOrCreateProjectAsync(string projectName)
    {
        var projects = await GetProjectsAsync();
        var project = projects.SingleOrDefault(p => p.Name == projectName) ?? await AddProjectAsync(projectName);
        return project ?? throw new NullReferenceException(nameof(project));
    }

    private static async Task DeleteTasksFromProjectAsync(string projectId, DateTime createdBeforeUtc)
    {
        var todoistTasks = await GetTasksFromProjectAsync(projectId);
        var idsToDelete = todoistTasks.Where(t => t.Parent_Id == null && t.Created_at < createdBeforeUtc).Select(t => t.Id);
        await Task.WhenAll(idsToDelete.Select(id => DeleteTaskAsync(id)).ToList());
    }

    private static string GetDueString(TrainingDayType trainingDayType) => $"every {string.Join(",",
        trainingDayType.DaysTraining.Select(d => TodoistDayDict.GetValueOrDefault(d)))}";

    private static Dictionary<Day, string> TodoistDayDict { get; } = new Dictionary<Day, string>
    {
        { Day.Sunday, "sun" },
        { Day.Monday, "mon" },
        { Day.Tuesday, "tue" },
        { Day.Wednesday, "wed" },
        { Day.Thursday, "thu" },
        { Day.Friday, "fri" },
        { Day.Saturday, "sat" },
    };
}
