using static SystemOfEquations.Todoist.TodoistApi;

namespace SystemOfEquations.Todoist;

internal class TodoistService
{
    public static async Task SyncAsync(Phase phase)
    {
        var allProjectsTask = GetProjectsAsync();
        var eatingProjectTask = GetOrCreateProjectAsync(allProjectsTask, "Eating");
        var cookingProjectTask = GetOrCreateProjectAsync(allProjectsTask, "Cooking");

        await Task.WhenAll(
            DeleteTasksFromProjectAsync(eatingProjectTask, createdBeforeUtc: DateTime.UtcNow),
            DeleteTasksFromProjectAsync(cookingProjectTask, createdBeforeUtc: DateTime.UtcNow),
            AddPhaseAsync(phase, eatingProjectTask, cookingProjectTask));
    }

    private static async Task AddServingAsync(TodoistTask parentTodoistTask, FoodServing s)
    {
        // Use polymorphic CreateTodoistSubtasksAsync method - no type checking needed
        // Cast AddTaskAsync to return object to work with public API
        await s.CreateTodoistSubtasksAsync(parentTodoistTask.Id,
            async (content, description, dueString, parentId, projectId) =>
                (object)await AddTaskAsync(content, description, dueString, parentId, projectId));
    }

    private static async Task AddServingsAsync(
        Task<Project> projectTask,
        string content,
        string? dueString,
        IEnumerable<FoodServing> servings)
    {
        var project = await projectTask;

        Console.WriteLine($"Adding task {content}...");
        var parentTodoistTask = await AddTaskAsync(
            content,
            description: null,
            dueString: dueString,
            parentId: null,
            project.Id,
            isCollapsed: true);
        Console.WriteLine($"Added task {content}...");
        await UpdateTaskCollapsedAsync(parentTodoistTask.Id, collapsed: true);
        await Task.WhenAll(servings.Select(s => AddServingAsync(parentTodoistTask, s)).ToList());
    }

    private static async Task AddMealPrepPlan(Task<Project> projectTask, MealPrepPlan m)
    {
        var project = await projectTask;

        Console.WriteLine($"Adding task {m.Name}...");
        var parentTodoistTask = await AddTaskAsync(
            m.Name, description: null, dueString: "every tue", parentId: null, project.Id, isCollapsed: true);
        Console.WriteLine($"Added task {m.Name}");
        await UpdateTaskCollapsedAsync(parentTodoistTask.Id, collapsed: true);

        // Create subtasks for each meal quantity - add sequentially to maintain order
        for (int mealCount = 1; mealCount <= m.MealCount; mealCount++)
        {
            var quantityLabel = mealCount == 1 ? "1 meal" : $"{mealCount} meals";
            await AddMealQuantitySubtask(parentTodoistTask, quantityLabel, m.Servings, mealCount, m.MealCount);
        }
    }

    private static async Task AddMealQuantitySubtask(
        TodoistTask parentTask,
        string quantityLabel,
        IEnumerable<FoodServing> baseServings,
        int mealCount,
        int totalMealCount)
    {
        Console.WriteLine($"Adding subtask {parentTask.Content} > {quantityLabel}...");
        var quantityTask = await AddTaskAsync(
            quantityLabel, description: null, dueString: null, parentTask.Id, projectId: null, isCollapsed: true);
        Console.WriteLine($"Added subtask {parentTask.Content} > {quantityLabel}");
        await UpdateTaskCollapsedAsync(quantityTask.Id, collapsed: true);

        // Scale servings based on meal count ratio
        decimal scaleFactor = (decimal)mealCount / totalMealCount;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();

        // Generate and add nutritional comment
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings);
        Console.WriteLine($"Adding comment for {parentTask.Content} > {quantityLabel}...");
        await AddCommentAsync(quantityTask.Id, comment);
        Console.WriteLine($"Added comment for {parentTask.Content} > {quantityLabel}");

        await Task.WhenAll(scaledServings.Select(s => AddServingAsync(quantityTask, s)));
    }

    private static async Task AddPhaseAsync(
        Phase phase, Task<Project> eatingProjectTask, Task<Project> cookingProjectTask)
    {
        List<Task> systemTasks =
        [
            .. phase.MealPrepPlan.MealPrepPlans.Select(m =>
                AddMealPrepPlan(cookingProjectTask, m)),
                    AddServingsAsync(
                            cookingProjectTask,
                            content: "Totals",
                            dueString: "every tues",
                            phase.MealPrepPlan.Total),
        ];

        foreach (var x in new[]
        {
            phase.TrainingWeek.XFitDay,
            phase.TrainingWeek.RunningDay,
            phase.TrainingWeek.NonworkoutDay,
        }.SelectMany(trainingDay => trainingDay.Meals.Select((meal, mealIdx) => new
        {
            DueString = GetDueString(trainingDay.TrainingDayType),
            Idx = mealIdx,
            trainingDay.TrainingDayType,
            Meal = meal,
        })).Where(x => x.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
        {
            var content = $"{x.Idx + 1} - {x.TrainingDayType} - {x.Meal.Name}";

            // Parent tasks need to be added in order so that they appear in order, so don't run them in parallel
            var eatingProject = await eatingProjectTask;
            Console.WriteLine($"Adding task {content}...");
            var parentTodoistTask = await AddTaskAsync(
                content, $"Synced on {DateTime.Now}",
                x.DueString, parentId: null, eatingProject.Id, isCollapsed: true);
            Console.WriteLine($"Added task {content}");
            await UpdateTaskCollapsedAsync(parentTodoistTask.Id, collapsed: true);

            // Add child tasks in parallel
            systemTasks.AddRange(x.Meal.Servings.Where(s => !s.IsConversion)
                .Select(async s =>
                {
                    Console.WriteLine($"Adding subtask {content} > {s}...");
                    await AddTaskAsync(
                        s.ToString(), description: null, dueString: null, parentTodoistTask.Id, projectId: null);
                    Console.WriteLine($"Added subtask {content} > {s}");
                }));

            var comment = TodoistServiceHelper.GenerateNutritionalComment(x.Meal.Servings);
            Console.WriteLine($"Adding comment for {content}...");
            systemTasks.Add(AddCommentAsync(parentTodoistTask.Id, comment));
            Console.WriteLine($"Added comment for {content}");
        }

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
        Console.WriteLine("Getting all projects...");
        var projects = await TodoistApi.GetProjectsAsync();
        Console.WriteLine("Got all projects");
        return projects;
    }

    private static async Task DeleteTasksFromProjectAsync(Task<Project> projectTask, DateTime createdBeforeUtc)
    {
        var project = await projectTask;
        Console.WriteLine($"Gettings tasks for {project.Name}...");
        var todoistTasks = await GetTasksFromProjectAsync(project.Id);
        Console.WriteLine($"Got tasks for {project.Name}...");

        var tasksToDelete = todoistTasks.Where(t => t.Parent_Id == null && t.Created_at < createdBeforeUtc);
        await Task.WhenAll(tasksToDelete.Select(async task =>
        {
            Console.WriteLine($"Deleting task {task.Content}...");
            await DeleteTaskAsync(task.Id);
            Console.WriteLine($"Deleted task {task.Content}");
        }).ToList());
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
