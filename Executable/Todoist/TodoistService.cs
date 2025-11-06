using static SystemOfEquations.Todoist.TodoistApi;
using SystemOfEquations.Data;

namespace SystemOfEquations.Todoist;

internal class TodoistService
{
    internal record DayTypeGroup(
        TrainingDayType TrainingDayType,
        TrainingDay TrainingDay,
        string DueString,
        List<MealWithIndex> Meals);

    internal record MealWithIndex(
        int Index,
        Meal Meal,
        IEnumerable<FoodServing> Servings);

    public static async Task SyncAsync(Phase phase)
    {
        // First, get projects and count operations needed
        var allProjects = await GetProjectsAsync();
        var eatingProject = await GetOrCreateProjectAsync(Task.FromResult(allProjects), "Eating");
        var cookingProject = await GetOrCreateProjectAsync(Task.FromResult(allProjects), "Cooking");

        var eatingTasks = await GetTasksFromProjectAsync(eatingProject.Id);
        var cookingTasks = await GetTasksFromProjectAsync(cookingProject.Id);

        var eatingTasksToDelete = eatingTasks.Where(t => t.Parent_Id == null && t.Created_at < DateTime.UtcNow).ToList();
        var cookingTasksToDelete = cookingTasks.Where(t => t.Parent_Id == null && t.Created_at < DateTime.UtcNow).ToList();

        // Count total operations
        int totalOperations = CalculateTotalOperations(phase, eatingTasksToDelete.Count, cookingTasksToDelete.Count);

        using var progress = new ProgressTracker(totalOperations);

        await Task.WhenAll(
            DeleteTasksFromProjectAsync(Task.FromResult(eatingProject), DateTime.UtcNow, progress),
            DeleteTasksFromProjectAsync(Task.FromResult(cookingProject), DateTime.UtcNow, progress),
            AddPhaseAsync(phase, Task.FromResult(eatingProject), Task.FromResult(cookingProject), progress));
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
        operations += 3 * 3; // Each group: parent task + collapse + comment

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

    private static async Task AddServingAsync(TodoistTask parentTodoistTask, FoodServing s, ProgressTracker progress)
    {
        await TodoistServiceHelper.CreateTodoistSubtasksAsync(s, parentTodoistTask.Id,
            async (content, description, dueString, parentId, projectId) =>
            {
                var task = await AddTaskAsync(content, description, dueString, parentId, projectId);
                progress.IncrementProgress();
                return (object)task;
            });
    }

    private static async Task AddServingsAsync(
        Task<Project> projectTask,
        string content,
        string? dueString,
        IEnumerable<FoodServing> servings,
        ProgressTracker progress,
        int? order = null)
    {
        var project = await projectTask;

        var parentTodoistTask = await AddTaskAsync(
            content,
            description: null,
            dueString: dueString,
            parentId: null,
            project.Id,
            isCollapsed: true,
            order: order);
        progress.IncrementProgress();

        await UpdateTaskCollapsedAsync(parentTodoistTask.Id, collapsed: true);
        progress.IncrementProgress();

        await Task.WhenAll(servings.Select(s => AddServingAsync(parentTodoistTask, s, progress)).ToList());
    }

    private static async Task AddMealPrepPlan(Task<Project> projectTask, MealPrepPlan m, int order, ProgressTracker progress)
    {
        var project = await projectTask;
        var hasCookingServings = m.CookingServings.Any();

        // Only create cooking task (eating servings handled separately in AddPhaseAsync)
        if (!hasCookingServings)
            return;

        var cookingTaskName = TodoistServiceHelper.GetMealPrepTaskName(m.Name);
        var cookingParentTask = await AddTaskAsync(
            cookingTaskName, description: null, dueString: "every tue", parentId: null, project.Id, isCollapsed: true, order: order);
        progress.IncrementProgress();

        await UpdateTaskCollapsedAsync(cookingParentTask.Id, collapsed: true);
        progress.IncrementProgress();

        // Create subtasks for each meal quantity
        for (int mealCount = 1; mealCount <= m.MealCount; mealCount++)
        {
            var quantityLabel = mealCount == 1 ? "1 meal" : $"{mealCount} meals";
            await AddMealQuantitySubtask(cookingParentTask, quantityLabel, m.CookingServings, mealCount, m.MealCount, m.TargetMacros, m.HasConversionFoods, progress);
        }
    }

    private static async Task AddMealQuantitySubtask(
        TodoistTask parentTask,
        string quantityLabel,
        IEnumerable<FoodServing> baseServings,
        int mealCount,
        int totalMealCount,
        Macros totalTargetMacros,
        bool hasConversionFoods,
        ProgressTracker progress)
    {
        var quantityTask = await AddTaskAsync(
            quantityLabel, description: null, dueString: null, parentTask.Id, projectId: null, isCollapsed: true);
        progress.IncrementProgress();

        await UpdateTaskCollapsedAsync(quantityTask.Id, collapsed: true);
        progress.IncrementProgress();

        // Scale servings based on meal count ratio
        decimal scaleFactor = (decimal)mealCount / totalMealCount;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();
        var scaledTargetMacros = totalTargetMacros * scaleFactor;

        // Generate and add nutritional comment in parallel with servings
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings, scaledTargetMacros, hasConversionFoods);
        await Task.WhenAll(
            scaledServings.Select(s => AddServingAsync(quantityTask, s, progress))
                .Append(AddCommentAsync(quantityTask.Id, comment).ContinueWith(_ => progress.IncrementProgress())));
    }

    private static async Task AddMealSubtask(
        TodoistTask dayTypeParentTask,
        MealWithIndex mealWithIndex,
        ProgressTracker progress)
    {
        var content = $"{mealWithIndex.Index} - {mealWithIndex.Meal.Name}";

        var mealTask = await AddTaskAsync(
            content,
            description: null,
            dueString: null,
            parentId: dayTypeParentTask.Id,
            projectId: null,
            isCollapsed: true);
        progress.IncrementProgress();

        await UpdateTaskCollapsedAsync(mealTask.Id, collapsed: true);
        progress.IncrementProgress();

        // For PrepareInAdvance meals, add food grouping name as first subtask
        if (mealWithIndex.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
        {
            await AddTaskAsync(
                mealWithIndex.Meal.FoodGrouping.Name,
                description: null,
                dueString: null,
                parentId: mealTask.Id,
                projectId: null);
            progress.IncrementProgress();
        }

        // Add servings as subtasks
        await Task.WhenAll(mealWithIndex.Servings.Select(s => AddServingAsync(mealTask, s, progress)));

        // Add nutritional comment with target macros
        // Use meal's HasConversionFoods property (mealWithIndex.Servings has conversion foods filtered out)
        var comment = TodoistServiceHelper.GenerateNutritionalComment(
            mealWithIndex.Servings,
            mealWithIndex.Meal.Macros,
            mealWithIndex.Meal.HasConversionFoods);
        await AddCommentAsync(mealTask.Id, comment);
        progress.IncrementProgress();
    }

    private static async Task AddPhaseAsync(
        Phase phase, Task<Project> eatingProjectTask, Task<Project> cookingProjectTask, ProgressTracker progress)
    {
        List<Task> systemTasks =
        [
            .. phase.MealPrepPlan.MealPrepPlans.Select((m, index) =>
                AddMealPrepPlan(cookingProjectTask, m, order: index + 1, progress)),
                    AddServingsAsync(
                            cookingProjectTask,
                            content: "Totals",
                            dueString: "every tues",
                            phase.MealPrepPlan.Total,
                            progress,
                            order: phase.MealPrepPlan.MealPrepPlans.Count() + 1),
        ];

        // Get day-type groups and create parent tasks with nested meals
        var dayTypeGroups = GetDayTypeGroups(phase).ToList();

        systemTasks.AddRange(dayTypeGroups.Select((group, groupIdx) => Task.Run(async () =>
        {
            var eatingProject = await eatingProjectTask;

            // Create parent task for the day type
            var dayTypeParentTask = await AddTaskAsync(
                content: group.TrainingDayType.ToString(),
                description: null,
                dueString: group.DueString,
                parentId: null,
                eatingProject.Id,
                isCollapsed: true,
                order: groupIdx + 1);
            progress.IncrementProgress();

            await UpdateTaskCollapsedAsync(dayTypeParentTask.Id, collapsed: true);
            progress.IncrementProgress();

            // Add comment with ACTUAL/TARGET macros for the day
            var dayComment = GenerateDayTypeComment(group.TrainingDay);
            await AddCommentAsync(dayTypeParentTask.Id, dayComment);
            progress.IncrementProgress();

            // Create meal subtasks
            foreach (var mealWithIndex in group.Meals)
            {
                await AddMealSubtask(dayTypeParentTask, mealWithIndex, progress);
            }
        })));

        await Task.WhenAll(systemTasks);
    }

    private static async Task<Project> GetOrCreateProjectAsync(Task<Project[]> projectsTask, string projectName)
    {
        var projects = await projectsTask;
        var project = projects.SingleOrDefault(p => p.Name == projectName) ?? await AddProjectAsync(projectName);
        return project ?? throw new NullReferenceException(nameof(project));
    }

    private static async Task<Project[]> GetProjectsAsync()
    {
        var projects = await TodoistApi.GetProjectsAsync();
        return projects;
    }

    private static async Task DeleteTasksFromProjectAsync(Task<Project> projectTask, DateTime createdBeforeUtc, ProgressTracker progress)
    {
        var project = await projectTask;
        var todoistTasks = await GetTasksFromProjectAsync(project.Id);

        var tasksToDelete = todoistTasks.Where(t => t.Parent_Id == null && t.Created_at < createdBeforeUtc);
        await Task.WhenAll(tasksToDelete.Select(async task =>
        {
            await DeleteTaskAsync(task.Id);
            progress.IncrementProgress();
        }).ToList());
    }

    private static string GenerateDayTypeComment(TrainingDay trainingDay)
    {
        var actualNutrients = trainingDay.ActualNutrients;

        if (trainingDay.HasConversionFoods)
        {
            return $"ACTUAL: {actualNutrients.Cals:F0} cals, {actualNutrients.Macros}, {actualNutrients.Fiber:F1}g fiber\nTARGET: {trainingDay.TargetMacros}";
        }
        else
        {
            return $"{actualNutrients.Cals:F0} cals, {actualNutrients.Macros}, {actualNutrients.Fiber:F1}g fiber";
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

            // Iterate through all meals in chronological order
            foreach (var meal in trainingDay.Meals)
            {
                IEnumerable<FoodServing> servings;

                if (meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
                {
                    // PrepareAsNeeded: show all servings (except conversions)
                    servings = meal.Servings.Where(s => !s.IsConversion);
                }
                else if (meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
                {
                    // PrepareInAdvance: show only AtEatingTime servings (except conversions)
                    // If no AtEatingTime servings, empty list (will show food grouping name only)
                    servings = meal.Servings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime && !s.IsConversion);
                }
                else
                {
                    continue; // Skip meals with unknown preparation method
                }

                mealsForDay.Add(new MealWithIndex(
                    Index: mealIndex++,
                    Meal: meal,
                    Servings: servings));
            }

            yield return new DayTypeGroup(
                TrainingDayType: trainingDay.TrainingDayType,
                TrainingDay: trainingDay,
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

    // Test helper - makes GenerateDayTypeComment accessible to tests
    internal static string GenerateDayTypeCommentPublic(TrainingDay trainingDay)
        => GenerateDayTypeComment(trainingDay);
}
