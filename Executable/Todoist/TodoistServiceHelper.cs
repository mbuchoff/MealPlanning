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
    public static int CountTodoistOperations(FoodServing serving)
    {
        // Check if it's a CompositeFoodServing
        if (serving is CompositeFoodServing composite)
        {
            // 1 for the composite parent task + all component operations
            return 1 + composite.GetComponentsForDisplay().Sum(CountTodoistOperations);
        }

        // Check if it's a StaticFoodServing - delegate to original
        if (serving is StaticFoodServing staticServing)
        {
            return CountTodoistOperations(staticServing.OriginalServing);
        }

        // Base FoodServing creates 1 task
        return 1;
    }
}
