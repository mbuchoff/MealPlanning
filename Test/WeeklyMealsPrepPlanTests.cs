using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;

namespace Test;

public class WeeklyMealsPrepPlanTests
{
    [Fact]
    public void WeeklyMealsPrepPlan_Total_Should_Aggregate_StaticServings_Correctly()
    {
        // Arrange - Create a simplified training week with known values
        var staticFood = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));

        var pFood = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Olive Oil",
            new(ServingUnits: 14, ServingUnits.Gram, Cals: 124, P: 0, F: 14, CTotal: 0, CFiber: 0));
        var cFood = new FoodServing("Sweet Potato",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 86, P: 1.6M, F: 0.1M, CTotal: 20, CFiber: 3));

        // Create a food grouping with static servings
        var staticServings = new List<FoodServing> { staticFood * 1.5M };
        var foodGrouping = new FoodGrouping(
            "Rice Bowl",
            staticServings,
            pFood,
            fFood,
            cFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        // Create meals with this grouping
        var meal1 = new Meal("Lunch", new Macros(P: 40, F: 15, C: 60), foodGrouping);
        var meal2 = new Meal("Dinner", new Macros(P: 35, F: 20, C: 55), foodGrouping);

        // Create meal prep plans simulating different training days
        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("XfitDay - Rice Bowl",
                meal1.Servings.Select(s => s * 2), 2), // 2 Xfit days
            new MealPrepPlan("RunningDay - Rice Bowl",
                meal2.Servings.Select(s => s * 3), 3), // 3 Running days
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var total = weeklyPlan.Total.ToList();

        // Assert
        // Find the brown rice in the total
        var riceServing = total.FirstOrDefault(s => s.Name == "Brown Rice");
        Assert.NotNull(riceServing);

        // Should be: 1.5 * 100g * (2 Xfit meals + 3 Running meals) = 150g * 5 = 750g
        Assert.Equal(750M, riceServing.NutritionalInformation.ServingUnits);
    }

    [Fact]
    public void WeeklyMealsPrepPlan_Total_Should_Combine_Like_Foods_Across_MealPrepPlans()
    {
        // Arrange
        var food1 = new FoodServing("Quinoa",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 120, P: 4.1M, F: 1.9M, CTotal: 21, CFiber: 2.8M));
        var food2 = new FoodServing("Black Beans",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 132, P: 8.9M, F: 0.5M, CTotal: 24, CFiber: 8.7M));

        // Create multiple meal prep plans with overlapping foods
        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Plan 1", new List<FoodServing>
            {
                food1 * 2,
                food2 * 1.5M
            }, 1),
            new MealPrepPlan("Plan 2", new List<FoodServing>
            {
                food1 * 3,
                food2 * 2.5M
            }, 1),
            new MealPrepPlan("Plan 3", new List<FoodServing>
            {
                food1 * 1.5M
                // No food2 in this plan
            }, 1)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var total = weeklyPlan.Total.OrderBy(s => s.Name).ToList();

        // Assert
        Assert.Equal(2, total.Count);

        var beansServing = total.First(s => s.Name == "Black Beans");
        Assert.Equal(400M, beansServing.NutritionalInformation.ServingUnits); // (1.5 + 2.5) * 100g

        var quinoaServing = total.First(s => s.Name == "Quinoa");
        Assert.Equal(650M, quinoaServing.NutritionalInformation.ServingUnits); // (2 + 3 + 1.5) * 100g
    }

    [Fact]
    public void CreateMealPrepPlan_Should_Scale_StaticServings_By_DaysEatingPreparedMeals()
    {
        // This test verifies that when creating a weekly meal prep plan from a training week,
        // all foods (including static servings) are correctly aggregated across all daily meal plans.
        // For example, if a food appears in meals for 3 different days, the total should reflect
        // the sum of servings needed for all those days.

        // This test uses the actual MuscleGain2 training week
        var trainingWeek = new MuscleGain2();
        var mealPrepPlan = WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek);

        // Act
        var total = mealPrepPlan.Total.ToList();

        // Assert
        // The total should contain all foods aggregated across all meal prep plans
        Assert.NotEmpty(total);

        // Verify that the total is the sum of all servings from all meal prep plans
        var allServingsFromPlans = mealPrepPlan.MealPrepPlans
            .SelectMany(plan => plan.Servings)
            .ToList();

        // Group by food name and sum servings (using same logic as CombineLikeServings)
        var expectedTotals = allServingsFromPlans
            .CombineLikeServings()
            .OrderBy(s => s.Name)
            .ToList();

        var actualTotals = total.OrderBy(s => s.Name).ToList();

        Assert.Equal(expectedTotals.Count, actualTotals.Count);

        for (int i = 0; i < expectedTotals.Count; i++)
        {
            Assert.Equal(expectedTotals[i].Name, actualTotals[i].Name);
            Assert.Equal(expectedTotals[i].NutritionalInformation.ServingUnits, actualTotals[i].NutritionalInformation.ServingUnits);
        }
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Include_Totals()
    {
        // Arrange
        var food = new FoodServing("Test Food",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 10, F: 5, CTotal: 15, CFiber: 2));

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Test Plan", new List<FoodServing> { food * 5 }, 1)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        Assert.Contains("Test Plan: 500 grams Test Food", output);
        Assert.Contains("Totals:", output);
        Assert.Contains("500 grams Test Food", output);
    }
    [Fact]
    public void SumWithSameFoodGrouping_Should_Return_MealCount()
    {
        // Arrange
        var foodGrouping = new FoodGrouping(
            "Test Grouping",
            [],
            new FoodServing("Protein", new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0)),
            new FoodServing("Fat", new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0)),
            new FoodServing("Carb", new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2)),
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var meals = new List<Meal>
        {
            new Meal("Meal 1", new Macros(P: 30, F: 15, C: 50), foodGrouping),
            new Meal("Meal 2", new Macros(P: 30, F: 15, C: 50), foodGrouping),
        };

        // Act
        var result = meals.SumWithSameFoodGrouping(2).ToList();

        // Assert
        Assert.Single(result);
        var (meal, mealCount) = result.First();
        Assert.Equal(4, mealCount); // 2 meals * 2 days = 4
        Assert.Equal("Test Grouping", meal.Name);
    }
}