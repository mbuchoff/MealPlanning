namespace SystemOfEquations;

internal record MealPrepPlan(IEnumerable<(string Name, IEnumerable<Helping> Helpings)> Meals)
{
    public override string ToString()
    {
        var helpingsStr = string.Join("\n", Meals.SelectMany(m => m.Helpings.Select(h => $"{m.Name}: {h}")));
        var totalStr = string.Join("\n", Total);
        return $"{helpingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<Helping> Total => Meals.SelectMany(m => m.Helpings).CombineLikeHelpings();
}

