namespace SystemOfEquations.Data;

public static class ServingUnits
{
    public static readonly ServingUnit None = new("", decimalsToDisplay: 1);
    public static readonly ServingUnit BlockTofu = new("block", decimalsToDisplay: 2);
    public static readonly ServingUnit Cup = new("cup", decimalsToDisplay: 1);
    public static readonly ServingUnit Gram = new("gram", decimalsToDisplay: 0);
    public static readonly ServingUnit Meal = new("meal", decimalsToDisplay: 0);
    public static readonly ServingUnit Scoop = new("scoop", decimalsToDisplay: 1, unitConversion: (0.380408M, Unit: Cup));
    public static readonly ServingUnit Tablespoon = new("tbsp", decimalsToDisplay: 1, unitConversion: (1M/16, Cup));
}
