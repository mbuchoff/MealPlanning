using static SystemOfEquations.Todoist.TodoistApi;

namespace SystemOfEquations.Todoist;

internal class TodoistService
{
    public static async Task SyncAsync(Phase phase)
    {
        var project = await GetOrCreateProjectAsync("Automation");

        await Task.WhenAll(
            DeleteTasksFromProjectAsync(project.Id, createdBeforeUtc: DateTime.UtcNow),
            AddPhaseAsync(phase, project.Id));
    }

    private static async Task AddPhaseAsync(Phase phase, string projectId)
    {
        var tasks = new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals.Select(meal => new
        {
            DueString = GetDueString(trainingDay.TrainingDayType),
            trainingDay.TrainingDayType,
            Meal = meal,
        })).Where(x => x.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
        .Select(async x =>
        {
            var content = $"{x.TrainingDayType} - {x.Meal.Name}";

            var parentTodoistTask = await AddTaskAsync(
                content, $"Synced on {DateTime.Now}",
                x.DueString, parentId: null, projectId);

            var childTasks = x.Meal.Helpings.Where(h => !h.Food.IsConversion).Select(async h =>
            {
                var subTask = await AddTaskAsync(
                    h.ToString(),
                    $"Synced on {DateTime.Now}",
                    dueString: null, parentTodoistTask.Id, projectId: null);
                await AddCommentAsync(subTask.Id, h.NutritionalInformation.ToNutrientsString());
            }).ToList();

            childTasks.Add(AddCommentAsync(parentTodoistTask.Id, x.Meal.NutritionalInformation.ToNutrientsString()));

            await Task.WhenAll(childTasks);
        }).ToList();

        await Task.WhenAll(tasks);
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
