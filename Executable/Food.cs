using System.Linq;

namespace SystemOfEquations;

/// <summary>
/// Represents a food with its equivalent serving sizes and nutritional information.
/// A food can be converted to different serving sizes while maintaining nutritional ratios.
/// </summary>
public record Food(
    string Name,
    (decimal Amount, ServingUnit Unit)[] ServingEquivalences,
    BaseNutrition Nutrition,
    FoodServing.AmountWater? Water = null,
    bool IsConversion = false)
{
    /// <summary>
    /// Creates a FoodServing instance with a specific serving size.
    /// </summary>
    public FoodServing WithServing(decimal amount, ServingUnit unit)
    {
        // Find a matching or compatible serving equivalence
        var exactMatch = ServingEquivalences.FirstOrDefault(e => e.Unit == unit);
        
        if (exactMatch != default)
        {
            // Calculate multiplier from the known equivalence
            var multiplier = amount / exactMatch.Amount;
            
            // Create NutritionalInformation with the serving size and scaled nutrition
            var scaledNutrition = new NutritionalInformation(
                amount, unit,
                Nutrition.Cals * multiplier,
                Nutrition.P * multiplier,
                Nutrition.F * multiplier,
                Nutrition.CTotal * multiplier,
                Nutrition.CFiber * multiplier);
            
            // Water calculation: Base stays constant, only PerServing is multiplied
            var scaledWater = Water == null ? null : 
                new FoodServing.AmountWater(Water.Base, Water.PerServing * multiplier);
            
            return new FoodServing(Name, scaledNutrition, scaledWater, IsConversion);
        }
        
        // Try to convert between compatible units (e.g., tbsp to cup)
        var compatible = ServingEquivalences.FirstOrDefault(
            e => e.Unit.UnitConversion.CentralUnit == unit.UnitConversion.CentralUnit);
        
        if (compatible != default)
        {
            // First create a food serving with the known equivalence
            var baseServing = WithServing(compatible.Amount, compatible.Unit);
            // Then convert to the desired unit
            return baseServing.Convert(unit);
        }
        
        throw new InvalidOperationException(
            $"Cannot create {amount} {unit} of {Name} - no compatible serving definition found. " +
            $"Available equivalences: {string.Join(", ", ServingEquivalences.Select(e => $"{e.Amount} {e.Unit}"))}");
    }
}