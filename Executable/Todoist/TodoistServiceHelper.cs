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
}
