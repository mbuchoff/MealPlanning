// ABOUTME: Tests to verify that fallback FoodGroupings are preserved during meal transformations
// such as cloning with tweaked macros and summing meals with the same food grouping
using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class MealFallbackTests
{
    [Fact]
    public void CloneWithTweakedMacros_Should_Preserve_All_Fallback_Options()
    {
        // Arrange - Create two fallback food groupings
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));

        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var foodGrouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var foodGrouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var originalMeal = new Meal("Test Meal", new Macros(P: 30, F: 15, C: 50),
            new FallbackChain(foodGrouping1, foodGrouping2));

        // Act - Clone with tweaked macros
        var clonedMeal = originalMeal.CloneWithTweakedMacros(1.1M, 1.0M, 1.0M);

        // Assert - Cloned meal should have both fallback options
        Assert.Equal(2, clonedMeal.FoodGroupings.Length);
        Assert.Same(foodGrouping1, clonedMeal.FoodGroupings[0]);
        Assert.Same(foodGrouping2, clonedMeal.FoodGroupings[1]);
    }

    [Fact]
    public void SumWithSameFoodGrouping_Should_Preserve_All_Fallback_Options()
    {
        // Arrange - Create meals with multiple fallback options
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));

        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var staticServing = new FoodServing("Static",
            new(ServingUnits: 50, ServingUnits.Gram, Cals: 50, P: 5, F: 2, CTotal: 10, CFiber: 1));

        var foodGrouping1 = new FoodGrouping("Grouping1", [staticServing * 1], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var foodGrouping2 = new FoodGrouping("Grouping2", [staticServing * 1], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var meals = new List<Meal>
        {
            new Meal("Meal 1", new Macros(P: 30, F: 15, C: 50), new FallbackChain(foodGrouping1, foodGrouping2)),
            new Meal("Meal 2", new Macros(P: 35, F: 20, C: 60), new FallbackChain(foodGrouping1, foodGrouping2)),
        };

        // Act - Sum meals
        var result = meals.SumWithSameFoodGrouping(2).ToList();

        // Assert - Summed meal should have both fallback options
        Assert.Single(result);
        var (summedMeal, mealCount) = result.First();
        Assert.Equal(2, summedMeal.FoodGroupings.Length);
        Assert.Equal(4, mealCount); // 2 meals * 2 days
    }

    [Fact]
    public void Meal_Should_Accept_FallbackChain_In_Constructor()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var fallbackChain = new FallbackChain(grouping1, grouping2);

        // Act
        var meal = new Meal("Test Meal", new Macros(P: 30, F: 15, C: 50), fallbackChain);

        // Assert
        Assert.NotNull(meal);
        Assert.Equal("Test Meal", meal.Name);
    }

    [Fact]
    public void WorkoutMeal_Should_Provide_Fallbacks_For_Conversions()
    {
        object workoutMeal = FoodGroupings.WorkoutMeal;

        var fallback = Assert.IsType<FallbackChain>(workoutMeal);
        Assert.Equal(3, fallback.Count);
        Assert.Equal(Foods.Edamame_1_Scoop, fallback.All[0].PFood);
        Assert.Equal(Foods.FatToCarbConversion, fallback.All[1].PFood);
        Assert.Equal(Foods.ProteinToCarbConversion, fallback.All[2].PFood);

        foreach (var grouping in fallback.All)
        {
            Assert.Equal(Foods.FatToCarbConversion, grouping.FFood);
            Assert.Equal(Foods.OrangeJuice_1_Cup, grouping.CFood);
            Assert.Equal("workout shake", grouping.Name);
            Assert.Equal(FoodGrouping.PreparationMethodEnum.PrepareAsNeeded, grouping.PreparationMethod);
        }
    }
}
