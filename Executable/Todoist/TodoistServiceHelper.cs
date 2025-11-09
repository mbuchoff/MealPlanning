namespace SystemOfEquations.Todoist;

using SystemOfEquations.Data;

internal static class TodoistServiceHelper
{
    public static string GenerateNutritionalComment(
        IEnumerable<FoodServing> servings,
        Macros? targetMacros = null,
        bool? hasConversionFoods = null)
    {
        var servingsList = servings.ToList();

        // Filter out conversion servings from individual breakdown
        var nonConversionServings = servingsList.Where(s => !s.IsConversion).ToList();

        // Calculate ACTUAL total (non-conversion servings only)
        var actualTotal = nonConversionServings
            .Select(s => s.NutritionalInformation)
            .Sum(1, ServingUnits.Meal);

        // Determine if there are conversion foods:
        // - If explicitly provided via flag, use that (for meal prep where conversion foods are filtered out)
        // - Otherwise, check the servings list (for inline meal comments)
        var hasConversions = hasConversionFoods ?? servingsList.Any(s => s.IsConversion);

        // Build header using shared formatting logic
        string headerText;

        // Special case: empty servings with targetMacros
        // This happens for PrepareInAdvance meals with no AtEatingTime servings
        // Show just the targetMacros without ACTUAL/TARGET split to avoid showing zeros
        if (nonConversionServings.Count == 0 && targetMacros != null)
        {
            headerText = targetMacros.ToString();
        }
        else if (hasConversions)
        {
            if (targetMacros != null)
            {
                // Show ACTUAL and TARGET when conversion foods + target macros
                // Use "\n" separator to match the format "ACTUAL:\n{actual}\n\nTARGET:\n{target}"
                headerText = NutritionalFormatting.FormatWithOptionalTarget(
                    $"\n{actualTotal.ToNutrientsString()}",
                    $"\n{targetMacros}",
                    hasConversionFoods: true,
                    prefix: "",
                    separator: "\n\n");
            }
            else
            {
                // Show ACTUAL and INTENDED when conversion foods without target macros
                var intendedTotal = servingsList
                    .Select(s => s.NutritionalInformation)
                    .Sum(1, ServingUnits.Meal);

                headerText = NutritionalFormatting.FormatWithOptionalTarget(
                    $"\n{actualTotal.ToNutrientsString()}",
                    $"\n{intendedTotal.ToNutrientsString()}",
                    hasConversionFoods: true,
                    prefix: "",
                    separator: "\n\n")
                    .Replace("TARGET:", "INTENDED:");
            }
        }
        else
        {
            // No conversion foods - show unlabeled total (ignore targetMacros)
            headerText = actualTotal.ToNutrientsString();
        }

        List<string> headerSections = new List<string> { headerText };

        // Create comment with header(s), then individual servings
        // Filter out zero-calorie foods (like water, creatine) from individual breakdown
        var comment = string.Join("\n\n",
            headerSections.Concat(
            nonConversionServings
                .Where(s => s.NutritionalInformation.Cals >= 1)
                .Select(s => $"{s.Name}\n{s.NutritionalInformation.ToNutrientsString()}")));

        return comment;
    }

    /// <summary>
    /// Gets the task name for a meal prep plan.
    /// </summary>
    public static string GetMealPrepTaskName(string mealName) => mealName;

    /// <summary>
    /// Formats a collection of servings as strings for display.
    /// This is the canonical formatting method used by both console output and Todoist.
    /// For composite servings, returns the flattened components.
    /// </summary>
    public static IEnumerable<string> FormatServingsAsStrings(IEnumerable<FoodServing> servings, string prefix = "")
    {
        foreach (var serving in servings)
        {
            // Get display components (flattens composites)
            var components = serving.GetComponentsForDisplay();
            foreach (var component in components)
            {
                yield return $"{prefix}{component}";
            }
        }
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
