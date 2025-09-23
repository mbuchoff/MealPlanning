using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;

namespace Test;

public class CompositeFoodServingTests
{
    private readonly FoodServing _yeast;
    private readonly FoodServing _gluten;
    private readonly CompositeFoodServing _composite;

    public CompositeFoodServingTests()
    {
        _yeast = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        _gluten = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        _composite = new CompositeFoodServing(
            "Seitan",
            new NutritionalInformation(1, ServingUnits.None, 79, 13.52M, 0.655M, 3.38M, 0.75M),
            [_yeast, _gluten],
            new FoodServing.AmountWater(0, 0.00916M));
    }

    [Fact]
    public void CompositeFoodServing_ToString_OutputsComponentsOnSeparateLines()
    {
        // Act
        var output = _composite.ToString();

        // Assert
        Assert.Contains("4 grams nutritional yeast", output);
        Assert.Contains("16 grams gluten", output);
        var lines = output.Split('\n');
        Assert.Equal(2, lines.Length); // Should output exactly 2 lines for 2 components
    }

    [Fact]
    public void CompositeFoodServing_Multiplication_ScalesComponents()
    {
        // Act
        var scaled = _composite * 2;
        var output = scaled.ToString();

        // Assert
        Assert.Contains("8 grams nutritional yeast", output);
        Assert.Contains("32 grams gluten", output);
    }

    [Fact]
    public void CompositeFoodServing_Multiplication_ScalesNutrition()
    {
        // Act
        var scaled = _composite * 2;

        // Assert
        Assert.Equal(158, scaled.NutritionalInformation.Cals);
        Assert.Equal(27.04M, scaled.NutritionalInformation.P);
        Assert.Equal(1.31M, scaled.NutritionalInformation.F);
        Assert.Equal(6.76M, scaled.NutritionalInformation.CTotal);
        Assert.Equal(1.5M, scaled.NutritionalInformation.CFiber);
    }

    [Fact]
    public void CompositeFoodServing_Water_StaysWithComposite()
    {
        // Assert that water is on composite but not components
        Assert.NotNull(_composite.Water);
        Assert.Equal(0.00916M, _composite.Water.PerServing);

        // Components should not have water
        foreach (var component in _composite.Components)
        {
            Assert.Null(component.Water); // Components should not have water
        }
    }

    [Fact]
    public void CompositeFoodServing_WithScaledServingUnits_OutputsCorrectly()
    {
        // Arrange - simulate what happens when CombineLikeServings creates a scaled composite
        var scaledComposite = new CompositeFoodServing(
            "Seitan",
            new NutritionalInformation(6.7M, ServingUnits.None, 529.3M, 90.584M, 4.3885M, 22.646M, 5.025M),
            [_yeast, _gluten],
            new FoodServing.AmountWater(0, 0.061372M));

        // Act
        var output = scaledComposite.ToString();

        // Assert - should output 6.7x scaled components (27 and 107 due to rounding)
        Assert.Contains("27 grams nutritional yeast", output);
        Assert.Contains("107 grams gluten", output);
    }

    [Fact]
    public void FromComponents_CalculatesNutritionAutomatically()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        // Act
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent],
            new FoodServing.AmountWater(0, 0.00916M));

        // Assert - nutrition should be sum of components
        Assert.Equal(79, composite.NutritionalInformation.Cals);  // 15 + 64
        Assert.Equal(13.52M, composite.NutritionalInformation.P);  // 1.25 + 12.27
        Assert.Equal(0.655M, composite.NutritionalInformation.F);  // 0.125 + 0.53
        Assert.Equal(3.38M, composite.NutritionalInformation.CTotal);  // 1.25 + 2.13
        Assert.Equal(0.75M, composite.NutritionalInformation.CFiber);  // 0.75 + 0
        Assert.Equal(1, composite.NutritionalInformation.ServingUnits);
        Assert.Equal(ServingUnits.None, composite.NutritionalInformation.ServingUnit);
    }

    [Fact]
    public void FromComponents_PreservesComponentsAndWater()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));
        var water = new FoodServing.AmountWater(0, 0.00916M);

        // Act
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent],
            water);

        // Assert
        Assert.Equal(2, composite.Components.Count);
        Assert.Equal(yeastComponent, composite.Components[0]);
        Assert.Equal(glutenComponent, composite.Components[1]);
        Assert.Equal(water, composite.Water);
        Assert.Equal("Auto-calculated Seitan", composite.Name);
    }

    [Fact]
    public void FromComponents_ToString_OutputsComponentsCorrectly()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        // Act
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent]);
        var output = composite.ToString();

        // Assert
        Assert.Contains("4 grams nutritional yeast", output);
        Assert.Contains("16 grams gluten", output);
    }

    [Fact]
    public void GetComponentsForDisplay_WithWater_IncludesWaterAsComponent()
    {
        // Arrange
        var water = new FoodServing.AmountWater(0, 0.0366666666666667M);
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten],
            water);

        // Act
        var components = composite.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(3, components.Count); // yeast, gluten, and water
        Assert.Contains(components, c => c.Name == "nutritional yeast");
        Assert.Contains(components, c => c.Name == "gluten");
        Assert.Contains(components, c => c.Name.Contains("water"));

        // Verify water component has correct amount
        var waterComponent = components.First(c => c.Name.Contains("water"));
        Assert.Equal("0.0 cups water", waterComponent.Name);
    }

    [Fact]
    public void GetComponentsForDisplay_WithWater_ScalesWaterCorrectly()
    {
        // Arrange
        var water = new FoodServing.AmountWater(0, 0.0366666666666667M);
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten],
            water);

        // Act - scale by 4x
        var scaled = composite * 4;

        // Debug: Check the scaled object's water and servings
        Assert.NotNull(scaled.Water);
        Assert.Equal(0.1466666666666668M, scaled.Water.PerServing, 10);
        Assert.Equal(4M, scaled.NutritionalInformation.ServingUnits);

        var components = scaled.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(3, components.Count);

        // Verify water component has scaled amount (0.0366666666666667 * 4 ≈ 0.147, rounds to 0.1)
        var waterComponent = components.First(c => c.Name.Contains("water"));
        // Water amount should be rounded to 0.1
        Assert.Equal("0.1 cups water", waterComponent.Name);
    }

    [Fact]
    public void GetComponentsForDisplay_NoWater_DoesNotIncludeWaterComponent()
    {
        // Arrange
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten],
            water: null);

        // Act
        var components = composite.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(2, components.Count); // only yeast and gluten, no water
        Assert.DoesNotContain(components, c => c.Name.Contains("water"));
    }
}
