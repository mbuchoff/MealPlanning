using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class WaterCalculationTests
{
    [Fact]
    public void BrownRice_Water_Should_Calculate_Correctly()
    {
        // The original behavior: 220 grams brown rice should have 3.9 cups water
        // Based on the formula: Base: 1.5M, PerServing: 0.5M
        // For 45g serving: 1.5 + 0.5 = 2.0 cups per 45g serving
        // For 220g: (220/45) servings = 4.889 servings
        // Expected water: 1.5 + (0.5 * 4.889) = 1.5 + 2.444 = 3.944 cups ≈ 3.9 cups
        
        var brownRice220g = Foods.BrownRice_45_Grams * (220M / 45M);
        
        // Check the water calculation
        Assert.NotNull(brownRice220g.Water);
        
        // For 220g of brown rice (4.889 servings of 45g)
        var expectedBase = 1.5M;
        var expectedPerServing = 0.5M * (220M / 45M);
        var expectedTotal = expectedBase + expectedPerServing;
        
        // Should be approximately 3.9 cups
        var actualTotal = brownRice220g.Water.Base + brownRice220g.Water.PerServing;
        Assert.Equal(3.9M, actualTotal, 1); // Allow 1 decimal place precision
    }
    
    [Fact]
    public void WheatBerries_Water_Should_Calculate_Correctly()
    {
        // Wheat berries have Water: new(Base: 2, PerServing: 0.8M)
        // For 45g serving: 2 + 0.8 = 2.8 cups per 45g serving
        
        var wheatBerries45g = Foods.WheatBerries_45_Grams;
        
        Assert.NotNull(wheatBerries45g.Water);
        var waterFor45g = wheatBerries45g.Water.Base + wheatBerries45g.Water.PerServing;
        Assert.Equal(2.8M, waterFor45g);
    }
    
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
        Assert.Equal(2.0M, oneServing.Water.Base + oneServing.Water.PerServing);
        
        // Two servings: Base should stay same, only PerServing doubles
        // Should be: 1.5 + (0.5 * 2) = 2.5, NOT (1.5 * 2) + (0.5 * 2) = 4.0
        Assert.Equal(1.5M, twoServings.Water.Base); // Base should NOT change
        Assert.Equal(1.0M, twoServings.Water.PerServing); // PerServing should double
        Assert.Equal(2.5M, twoServings.Water.Base + twoServings.Water.PerServing);
    }
}