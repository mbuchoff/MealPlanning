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

    public Food Copy(ServingUnit newServingUnit, double newServings) =>
        new(Name,
            new(newServings, newServingUnit,
                NutritionalInformation.Cals,
                NutritionalInformation.P,
                NutritionalInformation.F,
                NutritionalInformation.CTotal,
                NutritionalInformation.CFiber));

    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.ServingUnits)} {Name}" +
        $"{(Water == null ? "" : $", {(Water.Base + Water.PerServing * quantity):f1} cups water")}";

    public Food Convert(ServingUnit newServingUnit, AmountWater? water = null)
    {
        var servingUnit = NutritionalInformation.ServingUnit;

        if (servingUnit.UnitConversion.CentralUnit != newServingUnit.UnitConversion.CentralUnit)
        {
            throw new Exception(
                $"Cannot convert {Name} from {servingUnit} to {newServingUnit} because they have a different " +
                $"{nameof(newServingUnit.UnitConversion.CentralUnit)}.");
        }

        var multiplier = newServingUnit.UnitConversion.NumCentralUnitsInThisUnit /
            (NutritionalInformation.ServingUnits * servingUnit.UnitConversion.NumCentralUnitsInThisUnit);

        return new(Name, new(1, newServingUnit,
            NutritionalInformation.Cals * multiplier,
            NutritionalInformation.P * multiplier,
            NutritionalInformation.F * multiplier,
            NutritionalInformation.CTotal * multiplier,
            NutritionalInformation.CFiber * multiplier),
            water == null ? null : water * multiplier);
    }
}
