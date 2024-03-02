namespace SystemOfEquations;

public record ServingUnit(string UnitName, int DecimalsToDisplay)
{
    public override string ToString() => UnitName;
    public string ToString(double servings)
    {
        var servingsStr = servings.ToString($"F{DecimalsToDisplay}");
        var pluralOrSingularEnding = double.Parse(servingsStr) == 1 ? "" : "s";
        return $"{servingsStr} {UnitName}{pluralOrSingularEnding}";
    }
}
