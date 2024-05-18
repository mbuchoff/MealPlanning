namespace SystemOfEquations;

internal record MealPrepPlan(IEnumerable<(string Description, Helping Helping)> Helpings)
{
    public override string ToString()
    {
        var helpingsStr = string.Join("\n", Helpings.Select(h => h.Description == null ?
            h.Helping.ToString() :
            $"{h.Description}: {h.Helping}"));
        var totalStr = string.Join("\n", Total);
        return $"{helpingsStr}\n\nTotals:\n{totalStr}";
    }

    public IEnumerable<Helping> Total => Helpings.Select(x => x.Helping).CombineLikeHelpings();
}

