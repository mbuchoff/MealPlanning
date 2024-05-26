namespace SystemOfEquations;

internal record Food(
    string Name,
    NutritionalInformation NutritionalInformation,
    Food.AmountWater? Water = null,
    bool IsConversion = false)
{
    public record AmountWater(double Base, double PerServing)
    {
        public static AmountWater operator *(AmountWater amountWater, double d) =>
            new(amountWater.Base * d, amountWater.PerServing * d);
    }

    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.Servings)} {Name}" +
        $"{(Water == null ? "" : $", {(Water.Base + Water.PerServing * quantity):f1} cups water")}";

    public Food Convert(ServingUnit newServingUnit, AmountWater? water = null)
    {
        var servingUnit = NutritionalInformation.ServingUnit;

        if (servingUnit.UnitConversion.CentralUnit != newServingUnit.UnitConversion.CentralUnit)
        {
            throw new Exception($"{servingUnit} has a different central unit than {newServingUnit}.");
        }

        var multiplier = newServingUnit.UnitConversion.NumCentralUnitsInThisUnit /
            (NutritionalInformation.Servings * servingUnit.UnitConversion.NumCentralUnitsInThisUnit);

        return new(Name, new(1, newServingUnit,
            NutritionalInformation.Cals * multiplier,
            NutritionalInformation.P * multiplier,
            NutritionalInformation.F * multiplier,
            NutritionalInformation.CTotal * multiplier,
            NutritionalInformation.CFiber * multiplier),
            water == null ? null : water * multiplier);
    }
}
