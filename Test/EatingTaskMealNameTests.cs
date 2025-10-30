using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;

namespace Test;

public class EatingTaskMealNameTests
{
    [Fact]
    public void Meal_WithAtEatingTimeServings_ShouldUseFoodGroupingNameNotMealName()
    {
        // Arrange - Create a meal with timing name and food grouping name
        var englishMuffin = new FoodServing("Ezekiel english muffin",
            new NutritionalInformation(1, ServingUnits.None, 90, 6, 0.5M, 17, 3))
        {
            AddWhen = FoodServing.AddWhenEnum.AtEatingTime
        };

        var wheatBerries = new FoodServing("wheat berries",
            new NutritionalInformation(45, ServingUnits.Gram, 150, 6, 0.5M, 33, 6));

        var pumpkinSeeds = new FoodServing("pumpkin seeds",
            new NutritionalInformation(30, ServingUnits.Gram, 170, 9, 15, 3, 2));

        var conversionFood = new FoodServing("protein to carb conversion",
            new NutritionalInformation(1, ServingUnits.Gram, 0, -1, 0, 1, 0),
            IsConversion: true);

        var foodGrouping = new FoodGrouping(
            "wheat berries", // This is the food name that should appear in eating task
            [englishMuffin],
            wheatBerries,
            pumpkinSeeds,
            conversionFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var meal = new Meal(
            "40 minutes after workout", // This is the timing/meal name
            new Macros(P: 28, F: 10, C: 120),
            foodGrouping);

        // Act - Get the food grouping name
        var foodGroupingName = meal.FoodGrouping.Name;
        var mealName = meal.Name;

        // Assert - For eating tasks, we want the food name, not the timing
        Assert.Equal("wheat berries", foodGroupingName);
        Assert.Equal("40 minutes after workout", mealName);

        // The eating task subtask should use foodGroupingName ("wheat berries")
        // NOT mealName ("40 minutes after workout")
    }

    [Fact]
    public void FoodServing_CanBeMarkedAsAtEatingTime()
    {
        // Arrange
        var eatingServing = new FoodServing("english muffin",
            new NutritionalInformation(1, ServingUnits.None, 100, 10, 5, 15, 2))
        {
            AddWhen = FoodServing.AddWhenEnum.AtEatingTime
        };

        var cookingServing = new FoodServing("chicken",
            new NutritionalInformation(100, ServingUnits.Gram, 165, 31, 3.6M, 0, 0))
        {
            AddWhen = FoodServing.AddWhenEnum.WithMeal
        };

        // Act & Assert
        Assert.Equal(FoodServing.AddWhenEnum.AtEatingTime, eatingServing.AddWhen);
        Assert.Equal(FoodServing.AddWhenEnum.WithMeal, cookingServing.AddWhen);
    }
}
