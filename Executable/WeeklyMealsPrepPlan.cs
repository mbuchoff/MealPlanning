namespace SystemOfEquations;

internal record WeeklyMealsPrepPlan(IEnumerable<MealPrepPlan> MealPrepPlans)
{
    public override string ToString()
    {
        var helpingsStr = string.Join("\n", MealPrepPlans.SelectMany(m => m.Helpings.Select(h => $"{m.Name}: {h}")));
        var totalStr = string.Join("\n", Total);
        return $"{helpingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<Helping> Total => MealPrepPlans.SelectMany(m => m.Helpings).CombineLikeHelpings();
}
