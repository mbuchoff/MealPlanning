namespace SystemOfEquations;

public record MealPrepPlan(IEnumerable<Helping> Helpings)
{
    public override string ToString() => string.Join("\n", Helpings.Select(h => h.ToString()));
}

