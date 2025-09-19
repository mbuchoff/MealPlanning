namespace SystemOfEquations;

public record FoodServing(
    string Name,
    NutritionalInformation NutritionalInformation,
    FoodServing.AmountWater? Water = null,
    bool IsConversion = false)
{
    public record AmountWater(decimal Base, decimal PerServing)
    {
        // When scaling water, Base stays constant (it's the initial water needed),
        // only PerServing is multiplied by the number of servings
        public static AmountWater operator *(AmountWater amountWater, decimal d) =>
            new(amountWater.Base, amountWater.PerServing * d);
    }

    public FoodServing Copy(ServingUnit newServingUnit, decimal newServings) =>
        new(Name,
            new(newServings, newServingUnit,
                NutritionalInformation.Cals,
                NutritionalInformation.P,
                NutritionalInformation.F,
                NutritionalInformation.CTotal,
                NutritionalInformation.CFiber));

    public string ToString(decimal quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.ServingUnits)} {Name}" +
        $"{(Water == null ? "" : $", {Water.Base + Water.PerServing * quantity:f1} cups water")}";

    public FoodServing Convert(ServingUnit newServingUnit, AmountWater? water = null)
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
            water == null ? null : water * multiplier,
            IsConversion);
    }
    
    public static FoodServing operator *(FoodServing fs, decimal multiplier) =>
        fs with {
            NutritionalInformation = fs.NutritionalInformation * multiplier,
            Water = fs.Water == null ? null : new AmountWater(fs.Water.Base, fs.Water.PerServing * multiplier)
        };
    
    public override string ToString() => ToString(1);

    // Virtual methods for polymorphic display handling
    public virtual IEnumerable<string> ToOutputLines(string prefix = "")
    {
        yield return $"{prefix}{ToString()}";
    }

    public virtual IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        // Base FoodServing returns itself
        yield return this;
    }
}
