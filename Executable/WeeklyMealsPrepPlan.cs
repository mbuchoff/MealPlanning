namespace SystemOfEquations;

internal record WeeklyMealsPrepPlan(IEnumerable<MealPrepPlan> MealPrepPlans)
{
    public override string ToString()
    {
        var servingsStr = string.Join("\n", MealPrepPlans.SelectMany(m => m.Servings.Select(s => $"{m.Name}: {s}")));
        var totalStr = string.Join("\n", Total);
        return $"{servingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<FoodServing> Total => MealPrepPlans.SelectMany(m => m.Servings).CombineLikeServings();
}
