using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class WaterCalculationTests
{
    [Fact]
    public void Multiple_Servings_Should_Only_Scale_PerServing_Not_Base()
    {
        // The key insight: Base water should NOT be multiplied by serving count
        // Only PerServing should be multiplied

        // Create a test food with known water requirements
        var testFood = new Food("test rice",
            [(45, ServingUnits.Gram)],
            new BaseNutrition(Cals: 100, P: 5, F: 1, CTotal: 20, CFiber: 2),
            Water: new(Base: 1.5M, PerServing: 0.5M));

        var oneServing = testFood.WithServing(45, ServingUnits.Gram);
        var twoServings = testFood.WithServing(90, ServingUnits.Gram);

        // One serving: Base + PerServing = 1.5 + 0.5 = 2.0
        Assert.NotNull(oneServing.Water);
        Assert.Equal(2.0M, oneServing.Water.Base + oneServing.Water.PerServing);

        // Two servings: Base should stay same, only PerServing doubles
        // Should be: 1.5 + (0.5 * 2) = 2.5, NOT (1.5 * 2) + (0.5 * 2) = 4.0
        Assert.NotNull(twoServings.Water);
        Assert.Equal(1.5M, twoServings.Water.Base); // Base should NOT change
        Assert.Equal(1.0M, twoServings.Water.PerServing); // PerServing should double
        Assert.Equal(2.5M, twoServings.Water.Base + twoServings.Water.PerServing);
    }
}