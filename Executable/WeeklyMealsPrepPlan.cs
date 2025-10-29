namespace SystemOfEquations;

internal record WeeklyMealsPrepPlan(IEnumerable<MealPrepPlan> MealPrepPlans)
{
    public override string ToString()
    {
        // Use polymorphic ToOutputLines method - no type checking needed
        var servingsStr = string.Join("\n", MealPrepPlans.SelectMany(m =>
            m.CookingServings.Concat(m.EatingServings).SelectMany(s => s.ToOutputLines($"{m.Name}: "))));

        var totalStr = string.Join("\n", Total.SelectMany(s => s.ToOutputLines()));

        return $"{servingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<FoodServing> Total => MealPrepPlans
        .SelectMany(m => m.CookingServings.Concat(m.EatingServings))
        .SelectMany(s => s.GetComponentsForDisplay()) // Expand composites to components
        .CombineLikeServings(); // Then combine like components
}
