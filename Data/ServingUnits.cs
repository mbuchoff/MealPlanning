namespace SystemOfEquations.Data;

internal static class ServingUnits
{
    public static ServingUnit Apple = new("apple", decimalsToDisplay: 1);
    public static ServingUnit Apricot = new("apricot", decimalsToDisplay: 0);
    public static ServingUnit BlockTofu = new("block", decimalsToDisplay: 2);
    public static ServingUnit Cup = new("cup", decimalsToDisplay: 1);
    public static ServingUnit Gram = new("gram", decimalsToDisplay: 0);
    public static ServingUnit Meal = new("meal", decimalsToDisplay: 0);
    public static ServingUnit Scoop = new("scoop", decimalsToDisplay: 1, unitConversion: (0.380408M, Unit: Cup));
    public static ServingUnit Tablespoon = new("tbsp", decimalsToDisplay: 1, unitConversion: (1M/16, Cup));
}
