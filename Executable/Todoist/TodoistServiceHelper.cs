namespace SystemOfEquations.Todoist;

using SystemOfEquations.Data;

internal static class TodoistServiceHelper
{
    public static string GenerateNutritionalComment(IEnumerable<FoodServing> servings)
    {
        var servingsList = servings.ToList();

        // Filter out conversion servings from individual breakdown
        var nonConversionServings = servingsList.Where(s => !s.IsConversion).ToList();

        // Calculate ACTUAL total (non-conversion servings only)
        var actualTotal = nonConversionServings
            .Select(s => s.NutritionalInformation)
            .Sum(1, ServingUnits.Meal);

        // Calculate INTENDED total (all servings including conversions)
        var intendedTotal = servingsList
            .Select(s => s.NutritionalInformation)
            .Sum(1, ServingUnits.Meal);

        // Create comment with ACTUAL, INTENDED, then individual servings
        var comment = string.Join("\n\n",
            new[] {
                $"ACTUAL:\n{actualTotal.ToNutrientsString()}",
                $"INTENDED:\n{intendedTotal.ToNutrientsString()}"
            }.Concat(
            nonConversionServings.Select(s => $"{s.Name}\n{s.NutritionalInformation.ToNutrientsString()}")));

        return comment;
    }

    /// <summary>
    /// Counts how many Todoist task operations a serving will create.
    /// Handles CompositeFoodServing by counting parent + all components recursively.
    /// </summary>
    public static int CountTodoistOperations(FoodServing serving) => serving switch
    {
        CompositeFoodServing composite => 1 + composite.GetComponentsForDisplay().Sum(CountTodoistOperations),
        StaticFoodServing staticServing => CountTodoistOperations(staticServing.OriginalServing),
        _ => 1
    };

    /// <summary>
    /// Creates Todoist subtasks for a serving.
    /// Uses GetComponentsForDisplay to determine if serving has components.
    /// Returns the created task ID if a parent task was created, null otherwise.
    /// </summary>
    public static async Task<string?> CreateTodoistSubtasksAsync(
        FoodServing serving,
        string parentTaskId,
        Func<string, string?, string?, string?, string?, Task<object>> addTaskFunc)
    {
        var components = serving.GetComponentsForDisplay().ToList();

        // Base FoodServing returns itself - create single task
        if (components.Count == 1 && ReferenceEquals(components[0], serving))
        {
            await addTaskFunc(serving.ToString(), null, null, parentTaskId, null);
            return null;
        }

        // Single component different from parent - skip parent, process component directly
        if (components.Count == 1)
        {
            await CreateTodoistSubtasksAsync(components[0], parentTaskId, addTaskFunc);
            return null;
        }

        // CompositeFoodServing returns components - create parent + component hierarchy
        var compositeTask = await addTaskFunc(serving.Name, null, null, parentTaskId, null);

        // Extract task ID from the returned object
        var taskId = compositeTask.GetType().GetProperty("Id")?.GetValue(compositeTask)?.ToString();
        if (taskId == null)
            throw new InvalidOperationException("Could not get task ID from created task");

        // Add each component as a subtask of the composite
        foreach (var component in components)
        {
            await CreateTodoistSubtasksAsync(component, taskId, addTaskFunc);
        }

        return taskId;
    }
}
