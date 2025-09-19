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
    public void CompositeFoodServing_Components_AreReadOnly()
    {
        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<FoodServing>>(_composite.Components);
        Assert.Equal(2, _composite.Components.Count);
    }
}