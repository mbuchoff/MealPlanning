using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;
using System.Linq;

namespace Test;

public class AddWhenTests
{
    [Fact]
    public void FoodServing_DefaultAddWhen_IsWithMeal()
    {
        // Arrange & Act
        var food = new FoodServing("test food",
            new NutritionalInformation(100, ServingUnits.Gram, 100, 10, 5, 15, 2));

        // Assert
        Assert.Equal(FoodServing.AddWhenEnum.WithMeal, food.AddWhen);
    }

    [Fact]
    public void MealPrepPlan_PartitionsServings_ByCookingAndEating()
    {
        // Arrange
        var cookingFood = new FoodServing("chicken",
            new NutritionalInformation(100, ServingUnits.Gram, 165, 31, 3.6M, 0, 0))
        {
            AddWhen = FoodServing.AddWhenEnum.WithMeal
        };

        var eatingFood = new FoodServing("english muffin",
            new NutritionalInformation(1, ServingUnits.None, 120, 4, 1, 24, 2))
        {
            AddWhen = FoodServing.AddWhenEnum.AtEatingTime
        };

        var mealPrepPlan = new MealPrepPlan(
            "Test Meal",
            [cookingFood],
            [eatingFood],
            5,
            new Macros(P: 40, F: 15, C: 60));

        // Assert
        Assert.Single(mealPrepPlan.CookingServings);
        Assert.Single(mealPrepPlan.EatingServings);
        Assert.Equal("chicken", mealPrepPlan.CookingServings.First().Name);
        Assert.Equal("english muffin", mealPrepPlan.EatingServings.First().Name);
    }
}
