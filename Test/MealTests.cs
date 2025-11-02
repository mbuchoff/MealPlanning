using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class MealTests
{
    [Fact]
    public void ToString_Should_Include_Macros()
    {
        // Arrange
        // Use realistic macros that work with Oatmeal grouping
        // Oatmeal() has no conversion foods, so output will be unlabeled
        var targetMacros = new Macros(P: 30, F: 20, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Oatmeal());

        // Act
        var output = meal.ToString();

        // Assert
        // Should contain macros (unlabeled since no conversion foods)
        Assert.Contains("30 P", output);
        Assert.Contains("20 F", output);
        Assert.Contains("80 C", output);
    }

    [Fact]
    public void ToString_Should_Show_Target_With_Conversion_Foods()
    {
        // Arrange
        // Use Oatmeal with conversion foods to see TARGET label
        var targetMacros = new Macros(P: 30, F: 20, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Oatmeal(withEdamame: false));

        // Act
        var output = meal.ToString();

        // Assert
        // Should show ACTUAL and TARGET labels since there are conversion foods
        Assert.Contains("ACTUAL:", output);
        Assert.Contains("TARGET:", output);
        // Target should show the macros
        Assert.Contains("30 P", output);
        Assert.Contains("20 F", output);
        Assert.Contains("80 C", output);
    }

    [Fact]
    public void ToString_Should_Show_Servings_For_PrepareInAdvance_Meals()
    {
        // Arrange
        var targetMacros = new Macros(P: 50, F: 10, C: 80);
        var meal = new Meal("Cooked Meal", targetMacros, FoodGroupings.Seitan);

        // Act
        var output = meal.ToString();

        // Assert
        // Should show the meal name and food grouping
        Assert.Contains("Cooked Meal", output);
        Assert.Contains("seitan", output);

        // Should show the servings breakdown for PrepareInAdvance meals
        // These are ingredients in the Seitan FoodGrouping
        Assert.Contains("olive oil", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("grams", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToString_Should_Not_Show_Actual_Target_Labels_Without_Conversion_Foods()
    {
        // Arrange
        // Oatmeal() defaults to withEdamame=true, which uses real Edamame food (NOT a conversion food)
        var targetMacros = new Macros(P: 30, F: 20, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Oatmeal());

        // Act
        var output = meal.ToString();

        // Assert
        // Should NOT show "ACTUAL:" or "TARGET:" labels when there are no conversion foods
        Assert.DoesNotContain("ACTUAL:", output);
        Assert.DoesNotContain("TARGET:", output);

        // Should still show the macros (unlabeled)
        Assert.Contains("30 P", output);
        Assert.Contains("20 F", output);
        Assert.Contains("80 C", output);
    }

    [Fact]
    public void ToString_Should_Show_Actual_Target_Labels_With_Conversion_Foods()
    {
        // Arrange
        // Oatmeal(withEdamame: false) uses ProteinToCarbConversion (IS a conversion food)
        var targetMacros = new Macros(P: 30, F: 20, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Oatmeal(withEdamame: false));

        // Act
        var output = meal.ToString();

        // Assert
        // SHOULD show "ACTUAL:" and "TARGET:" labels when there are conversion foods
        Assert.Contains("ACTUAL:", output);
        Assert.Contains("TARGET:", output);
    }
}
