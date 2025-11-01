using static SystemOfEquations.Todoist.TodoistApi;
using SystemOfEquations.Data;

namespace SystemOfEquations.Todoist;

internal class TodoistService
{
    internal record DayTypeGroup(
        TrainingDayType TrainingDayType,
        string DueString,
        List<MealWithIndex> Meals);

    internal record MealWithIndex(
        int Index,
        Meal Meal,
        IEnumerable<FoodServing> Servings);

    public static async Task SyncAsync(Phase phase)
    {
        // Create batch for accumulating commands
        var batch = new CommandBatch();

        // First, get projects and count operations needed
        var allProjects = await GetProjectsAsync();
        var eatingProject = await GetOrCreateProjectAsync(Task.FromResult(allProjects), "Eating", batch);
        var cookingProject = await GetOrCreateProjectAsync(Task.FromResult(allProjects), "Cooking", batch);

        var eatingTasks = await GetTasksFromProjectAsync(eatingProject.Id);
        var cookingTasks = await GetTasksFromProjectAsync(cookingProject.Id);

        var eatingTasksToDelete = eatingTasks.Where(t => t.Parent_Id == null && t.Created_at < DateTime.UtcNow).ToList();
        var cookingTasksToDelete = cookingTasks.Where(t => t.Parent_Id == null && t.Created_at < DateTime.UtcNow).ToList();

        // Count total operations (now counting batch executions instead of individual API calls)
        int totalOperations = CalculateTotalOperations(phase, eatingTasksToDelete.Count, cookingTasksToDelete.Count);

        using var progress = new ProgressTracker(totalOperations);

        await DeleteTasksFromProjectAsync(Task.FromResult(eatingProject), DateTime.UtcNow, progress, batch);
        await DeleteTasksFromProjectAsync(Task.FromResult(cookingProject), DateTime.UtcNow, progress, batch);
        await AddPhaseAsync(phase, Task.FromResult(eatingProject), Task.FromResult(cookingProject), progress, batch);

        // Final flush of any remaining commands
        await FlushBatchAsync(batch, progress);
    }

    private static int CalculateTotalOperations(Phase phase, int eatingTasksToDelete, int cookingTasksToDelete)
    {
        int operations = 0;

        // Deletions
        operations += eatingTasksToDelete;
        operations += cookingTasksToDelete;

        // Meal prep plans (cooking only)
        foreach (var mealPrepPlan in phase.MealPrepPlan.MealPrepPlans)
        {
            var hasCookingServings = mealPrepPlan.CookingServings.Any();

            // Cooking task operations
            if (hasCookingServings)
            {
                operations++; // Main task
                operations++; // Collapse
                operations += mealPrepPlan.MealCount * 2; // Each quantity subtask + collapse
                operations += mealPrepPlan.MealCount; // Comments for each quantity

                // Servings for each quantity - use CountTodoistOperations to handle composites
                foreach (var serving in mealPrepPlan.CookingServings)
                {
                    operations += mealPrepPlan.MealCount * TodoistServiceHelper.CountTodoistOperations(serving);
                }
            }
        }

        // Totals
        operations++; // Main task
        operations++; // Collapse
        operations += phase.MealPrepPlan.Total.Sum(s => TodoistServiceHelper.CountTodoistOperations(s)); // All serving operations

        // Day-type parent tasks (3 groups: XFit, Running, NonWorkout)
        operations += 3 * 2; // Each group: parent task + collapse

        // Eating meals from PrepareInAdvance meals (AtEatingTime servings)
        var eatingTasksFromPrepMeals = new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals
            .Where(meal => meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance &&
                          meal.Servings.Any(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime)))
            .ToList();

        foreach (var meal in eatingTasksFromPrepMeals)
        {
            operations++; // Meal task
            operations++; // Collapse
            operations++; // Food grouping name subtask
            var eatingServings = meal.Servings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime && !s.IsConversion);
            operations += eatingServings.Sum(s => TodoistServiceHelper.CountTodoistOperations(s));
            operations++; // Comment
        }

        // Eating meals (PrepareAsNeeded)
        var eatingMeals = new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals
            .Where(meal => meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
            .ToList();

        foreach (var meal in eatingMeals)
        {
            operations++; // Meal task
            operations++; // Collapse
            operations += meal.Servings.Where(s => !s.IsConversion).Sum(s => TodoistServiceHelper.CountTodoistOperations(s));
            operations++; // Comment
        }

        return operations;
    }

    /// <summary>
    /// Flushes the batch if it has commands, executes them, and records temp ID mappings.
    /// Increments progress for each command executed.
    /// </summary>
    private static async Task FlushBatchAsync(CommandBatch batch, ProgressTracker progress)
    {
        if (batch.IsEmpty)
            return;

        var commandCount = batch.Count;
        var response = await TodoistApi.ExecuteBatchAsync(batch);
        batch.RecordTempIdMappings(response.TempIdMapping);
        batch.Clear();

        // Increment progress for each command executed
        for (int i = 0; i < commandCount; i++)
        {
            progress.IncrementProgress();
        }
    }

    /// <summary>
    /// Flushes the batch if it's full (at 100 command limit).
    /// </summary>
    private static async Task FlushIfFullAsync(CommandBatch batch, ProgressTracker progress)
    {
        if (batch.IsFull)
        {
            await FlushBatchAsync(batch, progress);
        }
    }

    private static async Task AddServingAsync(string parentTaskId, FoodServing s, ProgressTracker progress, CommandBatch batch)
    {
        await TodoistServiceHelper.CreateTodoistSubtasksAsync(s, parentTaskId,
            async (content, description, dueString, parentId, projectId) =>
            {
                var taskId = batch.AddItemAddCommand(content, description, projectId, parentId, dueString);
                await FlushIfFullAsync(batch, progress);
                return (object)taskId;
            });
    }

    private static async Task AddServingsAsync(
        Task<Project> projectTask,
        string content,
        string? dueString,
        IEnumerable<FoodServing> servings,
        ProgressTracker progress,
        CommandBatch batch,
        int? order = null)
    {
        var project = await projectTask;

        var parentTaskId = batch.AddItemAddCommand(
            content,
            description: null,
            projectId: project.Id,
            parentId: null,
            dueString: dueString,
            collapsed: null,
            childOrder: order);
        await FlushIfFullAsync(batch, progress);

        batch.AddItemUpdateCommand(parentTaskId, collapsed: true);
        await FlushIfFullAsync(batch, progress);

        foreach (var s in servings)
        {
            await AddServingAsync(parentTaskId, s, progress, batch);
        }
    }

    private static async Task AddMealPrepPlan(Task<Project> projectTask, MealPrepPlan m, int order, ProgressTracker progress, CommandBatch batch)
    {
        var project = await projectTask;
        var hasCookingServings = m.CookingServings.Any();
        var hasEatingServings = m.EatingServings.Any();

        // Only create cooking task (eating servings handled separately in AddPhaseAsync)
        if (!hasCookingServings)
            return;

        var cookingTaskName = hasEatingServings ? $"{m.Name} - Cooking" : m.Name;
        var cookingParentTaskId = batch.AddItemAddCommand(
            cookingTaskName,
            description: null,
            projectId: project.Id,
            parentId: null,
            dueString: "every tue",
            collapsed: null,
            childOrder: order);
        await FlushIfFullAsync(batch, progress);

        batch.AddItemUpdateCommand(cookingParentTaskId, collapsed: true);
        await FlushIfFullAsync(batch, progress);

        // Create subtasks for each meal quantity
        for (int mealCount = 1; mealCount <= m.MealCount; mealCount++)
        {
            var quantityLabel = mealCount == 1 ? "1 meal" : $"{mealCount} meals";
            await AddMealQuantitySubtask(cookingParentTaskId, quantityLabel, m.CookingServings, mealCount, m.MealCount, progress, batch);
        }
    }

    private static async Task AddMealQuantitySubtask(
        string parentTaskId,
        string quantityLabel,
        IEnumerable<FoodServing> baseServings,
        int mealCount,
        int totalMealCount,
        ProgressTracker progress,
        CommandBatch batch)
    {
        var quantityTaskId = batch.AddItemAddCommand(
            quantityLabel,
            description: null,
            projectId: null,
            parentId: parentTaskId,
            dueString: null,
            collapsed: null);
        await FlushIfFullAsync(batch, progress);

        batch.AddItemUpdateCommand(quantityTaskId, collapsed: true);
        await FlushIfFullAsync(batch, progress);

        // Scale servings based on meal count ratio
        decimal scaleFactor = (decimal)mealCount / totalMealCount;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();

        // Add servings
        foreach (var s in scaledServings)
        {
            await AddServingAsync(quantityTaskId, s, progress, batch);
        }

        // Add nutritional comment
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings);
        batch.AddNoteAddCommand(quantityTaskId, comment);
        await FlushIfFullAsync(batch, progress);
    }

    private static async Task AddMealSubtask(
        string dayTypeParentTaskId,
        MealWithIndex mealWithIndex,
        ProgressTracker progress,
        CommandBatch batch)
    {
        var content = $"{mealWithIndex.Index} - {mealWithIndex.Meal.Name}";

        var mealTaskId = batch.AddItemAddCommand(
            content,
            description: null,
            projectId: null,
            parentId: dayTypeParentTaskId,
            dueString: null,
            collapsed: null);
        await FlushIfFullAsync(batch, progress);

        batch.AddItemUpdateCommand(mealTaskId, collapsed: true);
        await FlushIfFullAsync(batch, progress);

        // For PrepareInAdvance meals, add food grouping name as first subtask
        if (mealWithIndex.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
        {
            batch.AddItemAddCommand(
                mealWithIndex.Meal.FoodGrouping.Name,
                description: null,
                projectId: null,
                parentId: mealTaskId,
                dueString: null);
            await FlushIfFullAsync(batch, progress);
        }

        // Add servings as subtasks
        foreach (var s in mealWithIndex.Servings)
        {
            await AddServingAsync(mealTaskId, s, progress, batch);
        }

        // Add nutritional comment
        var comment = TodoistServiceHelper.GenerateNutritionalComment(mealWithIndex.Servings);
        batch.AddNoteAddCommand(mealTaskId, comment);
        await FlushIfFullAsync(batch, progress);
    }

    private static async Task AddPhaseAsync(
        Phase phase, Task<Project> eatingProjectTask, Task<Project> cookingProjectTask, ProgressTracker progress, CommandBatch batch)
    {
        // Add meal prep plans
        foreach (var (m, index) in phase.MealPrepPlan.MealPrepPlans.Select((m, i) => (m, i)))
        {
            await AddMealPrepPlan(cookingProjectTask, m, order: index + 1, progress, batch);
        }

        // Add totals
        await AddServingsAsync(
            cookingProjectTask,
            content: "Totals",
            dueString: "every tues",
            phase.MealPrepPlan.Total,
            progress,
            batch,
            order: phase.MealPrepPlan.MealPrepPlans.Count() + 1);

        // Get day-type groups and create parent tasks with nested meals
        var dayTypeGroups = GetDayTypeGroups(phase).ToList();
        var eatingProject = await eatingProjectTask;

        foreach (var (group, groupIdx) in dayTypeGroups.Select((g, i) => (g, i)))
        {
            // Create parent task for the day type
            var dayTypeParentTaskId = batch.AddItemAddCommand(
                content: group.TrainingDayType.ToString(),
                description: null,
                projectId: eatingProject.Id,
                parentId: null,
                dueString: group.DueString,
                collapsed: null,
                childOrder: groupIdx + 1);
            await FlushIfFullAsync(batch, progress);

            batch.AddItemUpdateCommand(dayTypeParentTaskId, collapsed: true);
            await FlushIfFullAsync(batch, progress);

            // Create meal subtasks
            foreach (var mealWithIndex in group.Meals)
            {
                await AddMealSubtask(dayTypeParentTaskId, mealWithIndex, progress, batch);
            }
        }
    }

    private static async Task<Project> GetOrCreateProjectAsync(Task<Project[]> projectsTask, string projectName, CommandBatch batch)
    {
        var projects = await projectsTask;
        var project = projects.SingleOrDefault(p => p.Name == projectName);

        if (project == null)
        {
            project = await AddProjectAsync(projectName);
        }

        return project ?? throw new NullReferenceException(nameof(project));
    }

    private static async Task<Project[]> GetProjectsAsync()
    {
        var projects = await TodoistApi.GetProjectsAsync();
        return projects;
    }

    private static async Task DeleteTasksFromProjectAsync(Task<Project> projectTask, DateTime createdBeforeUtc, ProgressTracker progress, CommandBatch batch)
    {
        var project = await projectTask;
        var todoistTasks = await GetTasksFromProjectAsync(project.Id);

        var tasksToDelete = todoistTasks.Where(t => t.Parent_Id == null && t.Created_at < createdBeforeUtc);
        foreach (var task in tasksToDelete)
        {
            batch.AddItemDeleteCommand(task.Id);
            await FlushIfFullAsync(batch, progress);
        }
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

    private static IEnumerable<DayTypeGroup> GetDayTypeGroups(Phase phase)
    {
        var allDayTypes = new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay
        };

        foreach (var trainingDay in allDayTypes)
        {
            var mealsForDay = new List<MealWithIndex>();
            int mealIndex = 1;

            // Collect PrepareAsNeeded meals
            foreach (var meal in trainingDay.Meals
                .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
            {
                mealsForDay.Add(new MealWithIndex(
                    Index: mealIndex++,
                    Meal: meal,
                    Servings: meal.Servings.Where(s => !s.IsConversion)));
            }

            // Collect PrepareInAdvance meals with AtEatingTime servings
            foreach (var meal in trainingDay.Meals
                .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance
                         && m.Servings.Any(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime)))
            {
                mealsForDay.Add(new MealWithIndex(
                    Index: mealIndex++,
                    Meal: meal,
                    Servings: meal.Servings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime && !s.IsConversion)));
            }

            yield return new DayTypeGroup(
                TrainingDayType: trainingDay.TrainingDayType,
                DueString: GetDueString(trainingDay.TrainingDayType),
                Meals: mealsForDay);
        }
    }

    // Test helper - makes GetDayTypeGroups accessible to tests
    internal static IEnumerable<DayTypeGroup> GetDayTypeGroupsPublic(Phase phase)
        => GetDayTypeGroups(phase);

    // Test helper - makes CalculateTotalOperations accessible to tests
    internal static int CalculateTotalOperationsPublic(Phase phase, int eatingTasksToDelete, int cookingTasksToDelete)
        => CalculateTotalOperations(phase, eatingTasksToDelete, cookingTasksToDelete);
}
