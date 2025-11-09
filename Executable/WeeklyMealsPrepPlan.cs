using SystemOfEquations.Data;
using SystemOfEquations.Extensions;
using SystemOfEquations.Todoist;

namespace SystemOfEquations;

internal record WeeklyMealsPrepPlan(IEnumerable<MealPrepPlan> MealPrepPlans)
{
    public override string ToString()
    {
        var mealPlanStrings = MealPrepPlans.Select(m =>
        {
            // Calculate ACTUAL macros from servings
            var actualNutrition = m.CookingServings.Concat(m.EatingServings)
                .Where(s => !s.IsConversion)
                .Select(s => s.NutritionalInformation)
                .Sum(1, ServingUnits.Meal);

            // Build header using shared formatting logic
            var macroLine = NutritionalFormatting.FormatWithOptionalTarget(
                actualNutrition.ToNutrientsString(),
                m.TargetMacros.ToString(),
                m.HasConversionFoods,
                prefix: "  ");
            var header = $"{m.Name}:\n{macroLine}";

            // Use shared formatting logic from TodoistServiceHelper
            var servings = string.Join("\n",
                TodoistServiceHelper.FormatServingsAsStrings(m.CookingServings.Concat(m.EatingServings)));

            return $"{header}\n{servings}";
        });

        var servingsStr = string.Join("\n\n", mealPlanStrings);

        // Use shared formatting logic for totals
        var totalStr = string.Join("\n", TodoistServiceHelper.FormatServingsAsStrings(Total));

        return $"{servingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<FoodServing> Total => MealPrepPlans
        .SelectMany(m => m.CookingServings.Concat(m.EatingServings))
        .SelectMany(s => s.GetComponentsForDisplay()) // Expand composites to components
        .CombineLikeServings(); // Then combine like components
}
