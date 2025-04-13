namespace SystemOfEquations;

public class ServingUnit
{
    public ServingUnit(string unitName, int decimalsToDisplay,
        (decimal NumCentralUnitsInThisUnit, ServingUnit Unit)? unitConversion = null)
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
    public (decimal NumCentralUnitsInThisUnit, ServingUnit CentralUnit) UnitConversion { get; }

    public override string ToString() => UnitName;
    public string ToString(decimal servings)
    {
        var servingsStr = servings.ToString($"F{DecimalsToDisplay}");
        var pluralOrSingularEnding = decimal.Parse(servingsStr) == 1 ? "" : "s";
        return $"{servingsStr} {UnitName}{pluralOrSingularEnding}";
    }
}
