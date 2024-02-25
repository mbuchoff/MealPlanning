namespace SystemOfEquations;

public record MealPrepPlan(IEnumerable<(string? Description, Helping Helping)> Helpings)
{
    public override string ToString() => string.Join("\n", Helpings.Select(h => h.Description == null ?
    h.Helping.ToString() :
    $"{h.Description} - {h.Helping}"));
}

