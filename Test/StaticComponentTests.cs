using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;
using System.Linq;

namespace Test;

public class StaticComponentTests
{
    private readonly FoodServing _protein;
    private readonly FoodServing _vegetable;
    private readonly FoodServing _salt;
    private readonly FoodServing _spice;

    public StaticComponentTests()
    {
        // Scalable components
        _protein = new FoodServing("chicken breast",
            new NutritionalInformation(100, ServingUnits.Gram, 165, 31, 3.6M, 0, 0));
        _vegetable = new FoodServing("broccoli",
            new NutritionalInformation(100, ServingUnits.Gram, 34, 2.8M, 0.4M, 6.6M, 2.6M));

        // Static components (seasonings)
        _salt = new FoodServing("salt",
            new NutritionalInformation(1, ServingUnits.Tablespoon, 0, 0, 0, 0, 0));
        _spice = new FoodServing("black pepper",
            new NutritionalInformation(0.5M, ServingUnits.Tablespoon, 3, 0.1M, 0, 0.6M, 0.3M));
    }

    [Fact]
    public void StaticFoodServing_WhenMultiplied_RemainsUnchanged()
    {
        // Arrange
        var staticServing = new StaticFoodServing(_salt);

        // Act
        var multiplied = staticServing * 3;

        // Assert
        Assert.Equal(1, multiplied.NutritionalInformation.ServingUnits);
        Assert.Equal(ServingUnits.Tablespoon, multiplied.NutritionalInformation.ServingUnit);
        Assert.Equal("salt", multiplied.Name);
    }

    [Fact]
    public void StaticFoodServing_PreservesOriginalServing()
    {
        // Arrange
        var staticServing = new StaticFoodServing(_spice);

        // Act
        var original = staticServing.OriginalServing;

        // Assert
        Assert.Equal(_spice, original);
        Assert.Equal(0.5M, original.NutritionalInformation.ServingUnits);
    }

    [Fact]
    public void CompositeFoodServing_WithMixedComponents_ScalesCorrectly()
    {
        // Arrange
        var staticSalt = new StaticFoodServing(_salt);
        var staticSpice = new StaticFoodServing(_spice);

        var composite = CompositeFoodServing.FromComponents(
            "Seasoned Meal",
            [_protein, _vegetable, staticSalt, staticSpice]);

        // Act
        var doubled = composite * 2;
        var components = doubled.GetComponentsForDisplay().ToList();

        // Assert - scalable components should be doubled
        Assert.Contains(components, c => c.Name == "chicken breast" &&
            c.NutritionalInformation.ServingUnits == 200); // 100 * 2
        Assert.Contains(components, c => c.Name == "broccoli" &&
            c.NutritionalInformation.ServingUnits == 200); // 100 * 2

        // Assert - static components should remain unchanged
        Assert.Contains(components, c => c.Name == "salt" &&
            c.NutritionalInformation.ServingUnits == 1); // stays 1
        Assert.Contains(components, c => c.Name == "black pepper" &&
            c.NutritionalInformation.ServingUnits == 0.5M); // stays 0.5
    }

    [Fact]
    public void CompositeFoodServing_ToString_DisplaysMixedComponentsCorrectly()
    {
        // Arrange
        var staticSalt = new StaticFoodServing(_salt);
        var composite = CompositeFoodServing.FromComponents(
            "Seasoned Chicken",
            [_protein, staticSalt]);

        // Act
        var singleOutput = composite.ToString();
        var doubledOutput = (composite * 2).ToString();

        // Assert - single serving
        Assert.Contains("100 grams chicken breast", singleOutput);
        Assert.Contains("1.0 tbsp salt", singleOutput);

        // Assert - doubled serving
        Assert.Contains("200 grams chicken breast", doubledOutput);
        Assert.Contains("1.0 tbsp salt", doubledOutput); // Salt stays the same
    }

    [Fact]
    public void CompositeFoodServing_NutritionalInfo_CalculatesCorrectlyWithStatic()
    {
        // Arrange
        var staticSalt = new StaticFoodServing(_salt);
        var staticSpice = new StaticFoodServing(_spice);

        var composite = CompositeFoodServing.FromComponents(
            "Seasoned Meal",
            [_protein, _vegetable, staticSalt, staticSpice]);

        // Act
        var nutrition = composite.NutritionalInformation;

        // Assert - total nutrition should be sum of all components
        Assert.Equal(165 + 34 + 0 + 3, nutrition.Cals); // 202
        Assert.Equal(31 + 2.8M + 0 + 0.1M, nutrition.P); // 33.9
        Assert.Equal(3.6M + 0.4M + 0 + 0, nutrition.F); // 4.0
        Assert.Equal(0 + 6.6M + 0 + 0.6M, nutrition.CTotal); // 7.2
        Assert.Equal(0 + 2.6M + 0 + 0.3M, nutrition.CFiber); // 2.9
    }

    [Fact]
    public void FromComponentsWithStatic_CreatesCorrectStructure()
    {
        // Arrange & Act
        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "Seasoned Meal",
            scalableComponents: [_protein, _vegetable],
            staticComponents: [_salt, _spice]);

        // Assert
        Assert.Equal(4, composite.Components.Count);

        // Verify static components are wrapped
        Assert.IsType<FoodServing>(composite.Components[0]); // protein - regular
        Assert.IsType<FoodServing>(composite.Components[1]); // vegetable - regular
        Assert.IsType<StaticFoodServing>(composite.Components[2]); // salt - static
        Assert.IsType<StaticFoodServing>(composite.Components[3]); // spice - static
    }

    [Fact]
    public void StaticFoodServing_ToOutputLines_RemainsConstant()
    {
        // Arrange
        var staticServing = new StaticFoodServing(_salt);

        // Act
        var lines1x = staticServing.ToOutputLines().ToList();
        var lines3x = (staticServing * 3).ToOutputLines().ToList();

        // Assert
        Assert.Single(lines1x);
        Assert.Single(lines3x);
        Assert.Equal(lines1x[0], lines3x[0]); // Output should be identical
        Assert.Contains("1.0 tbsp salt", lines1x[0]);
    }

    [Fact]
    public void CompositeFoodServing_WithOnlyStaticComponents_NeverScales()
    {
        // Arrange
        var staticSalt = new StaticFoodServing(_salt);
        var staticSpice = new StaticFoodServing(_spice);

        var composite = CompositeFoodServing.FromComponents(
            "Spice Mix",
            [staticSalt, staticSpice]);

        // Act
        var tripled = composite * 3;
        var output = tripled.ToString();

        // Assert - all components remain at original serving
        Assert.Contains("1.0 tbsp salt", output);
        Assert.Contains("0.5 tbsps black pepper", output);
        Assert.DoesNotContain("3.0 tbsp", output);
        Assert.DoesNotContain("1.5 tbsp", output);
    }

    [Fact]
    public void CompositeFoodServing_ComplexNesting_HandlesStaticCorrectly()
    {
        // Arrange - create a nested composite with static components
        var innerStatic = new StaticFoodServing(_salt);
        var innerComposite = CompositeFoodServing.FromComponents(
            "Seasoned Protein",
            [_protein, innerStatic]);

        var outerStatic = new StaticFoodServing(_spice);
        var outerComposite = CompositeFoodServing.FromComponents(
            "Full Meal",
            [innerComposite, _vegetable, outerStatic]);

        // Act
        var doubled = outerComposite * 2;
        var output = doubled.ToString();

        // Assert
        Assert.Contains("200 grams chicken breast", output); // Scaled
        Assert.Contains("200 grams broccoli", output); // Scaled
        Assert.Contains("1.0 tbsp salt", output); // Not scaled
        Assert.Contains("0.5 tbsps black pepper", output); // Not scaled
    }

}