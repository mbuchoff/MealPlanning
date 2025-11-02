using SystemOfEquations.Data;
using SystemOfEquations.Extensions;

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

            // Build header - show ACTUAL/TARGET labels only if there were conversion foods
            string header;
            if (m.HasConversionFoods)
            {
                header = $"{m.Name}:\n  ACTUAL: {actualNutrition.ToNutrientsString()}\n  TARGET: {m.TargetMacros}";
            }
            else
            {
                // No conversion foods - show unlabeled macros
                header = $"{m.Name}:\n  {actualNutrition.ToNutrientsString()}";
            }
            var servings = string.Join("\n", m.CookingServings.Concat(m.EatingServings)
                .SelectMany(s => s.ToOutputLines()));

            return $"{header}\n{servings}";
        });

        var servingsStr = string.Join("\n\n", mealPlanStrings);
        var totalStr = string.Join("\n", Total.SelectMany(s => s.ToOutputLines()));

        return $"{servingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<FoodServing> Total => MealPrepPlans
        .SelectMany(m => m.CookingServings.Concat(m.EatingServings))
        .SelectMany(s => s.GetComponentsForDisplay()) // Expand composites to components
        .CombineLikeServings(); // Then combine like components
}
