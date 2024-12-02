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

    public string ToString(ServingUnit servingUnit, double quantity)
    {
        var servingInfo = NutritionalInformation.ServingInfo.Single(si => si.ServingUnit ==  servingUnit);

        return
            $"{servingInfo.ServingUnit.ToString(quantity * servingInfo.Servings)} {Name}" +
            $"{(Water == null ? "" : $", {(Water.Base + Water.PerServing * quantity):f1} cups water")}";
    }

    public Food Convert(ServingUnit newServingUnit, AmountWater? water = null)
    {
        var servingInfos = NutritionalInformation.ServingInfo
            .Where(si => si.ServingUnit.UnitConversion.CentralUnit == newServingUnit.UnitConversion.CentralUnit)
            .ToList();

        if (servingInfos.Count == 0)
        {
            throw new Exception(
                $"Cannot convert {Name} to {newServingUnit} because there is no matching central unit: " +
                $"{nameof(newServingUnit.UnitConversion.CentralUnit)}.");
        }

        (var servings, var servingUnit) = servingInfos.First();

        var multiplier = newServingUnit.UnitConversion.NumCentralUnitsInThisUnit /
            (servings * servingUnit.UnitConversion.NumCentralUnitsInThisUnit);

        return new(Name, new([(1, newServingUnit)],
            NutritionalInformation.Cals * multiplier,
            NutritionalInformation.P * multiplier,
            NutritionalInformation.F * multiplier,
            NutritionalInformation.CTotal * multiplier,
            NutritionalInformation.CFiber * multiplier),
            water == null ? null : water * multiplier);
    }
}
