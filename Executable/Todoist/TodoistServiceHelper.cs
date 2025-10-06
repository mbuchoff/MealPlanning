namespace SystemOfEquations.Todoist;

using SystemOfEquations.Data;

internal static class TodoistServiceHelper
{
    public static string GenerateNutritionalComment(IEnumerable<FoodServing> servings)
    {
        // Filter out conversion servings for individual breakdown
        var nonConversionServings = servings.Where(s => !s.IsConversion).ToList();

        // Calculate total nutritional information from all servings (including conversions)
        var totalNutritionalInfo = servings
            .Select(s => s.NutritionalInformation)
            .Sum(1, ServingUnits.Meal);

        // Create comment with total first, then individual servings
        var comment = string.Join("\n\n",
            new[] { totalNutritionalInfo.ToNutrientsString() }.Concat(
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
    /// Handles CompositeFoodServing by creating parent + component hierarchy.
    /// Returns the created task ID if a parent task was created, null otherwise.
    /// </summary>
    public static async Task<string?> CreateTodoistSubtasksAsync(
        FoodServing serving,
        string parentTaskId,
        Func<string, string?, string?, string?, string?, Task<object>> addTaskFunc)
    {
        // Check if it's a CompositeFoodServing
        if (serving is CompositeFoodServing composite)
        {
            // Create a parent task for the composite food
            var compositeTask = await addTaskFunc(composite.Name, null, null, parentTaskId, null);

            // Extract task ID from the returned object
            var taskId = compositeTask.GetType().GetProperty("Id")?.GetValue(compositeTask)?.ToString();
            if (taskId == null)
                throw new InvalidOperationException("Could not get task ID from created task");

            // Add each component as a subtask of the composite
            foreach (var component in composite.GetComponentsForDisplay())
            {
                await CreateTodoistSubtasksAsync(component, taskId, addTaskFunc);
            }

            return taskId;
        }

        // Check if it's a StaticFoodServing - delegate to original
        if (serving is StaticFoodServing staticServing)
        {
            return await CreateTodoistSubtasksAsync(staticServing.OriginalServing, parentTaskId, addTaskFunc);
        }

        // Base FoodServing creates a single subtask
        await addTaskFunc(serving.ToString(), null, null, parentTaskId, null);
        return null;
    }
}
