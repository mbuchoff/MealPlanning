namespace SystemOfEquations;

public class ServingUnit
{
    public ServingUnit(string unitName, int decimalsToDisplay,
        (double NumCentralUnitsInThisUnit, ServingUnit Unit)? unitConversion = null)
    {
        UnitName = unitName;
        DecimalsToDisplay = decimalsToDisplay;

        if (unitConversion == null)
        {
            UnitConversion = (NumCentralUnitsInThisUnit: 1, CentralUnit: this);
        }
        else
        {
            UnitConversion = unitConversion.Value;
        }
    }

    public string UnitName { get; }
    public int DecimalsToDisplay { get; }
    public (double NumCentralUnitsInThisUnit, ServingUnit CentralUnit) UnitConversion { get; }

    public override string ToString() => UnitName;
    public string ToString(double servings)
    {
        var servingsStr = servings.ToString($"F{DecimalsToDisplay}");
        var pluralOrSingularEnding = double.Parse(servingsStr) == 1 ? "" : "s";
        return $"{servingsStr} {UnitName}{pluralOrSingularEnding}";
    }
}
