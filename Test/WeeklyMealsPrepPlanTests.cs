using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;

namespace Test;

public class WeeklyMealsPrepPlanTests
{
    [Fact]
    public void WeeklyMealsPrepPlan_Total_Should_Aggregate_StaticHelpings_Correctly()
    {
        // Arrange - Create a simplified training week with known values
        var staticFood = new Food("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        
        var pFood = new Food("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var fFood = new Food("Olive Oil",
            new(ServingUnits: 14, ServingUnits.Gram, Cals: 124, P: 0, F: 14, CTotal: 0, CFiber: 0));
        var cFood = new Food("Sweet Potato",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 86, P: 1.6M, F: 0.1M, CTotal: 20, CFiber: 3));
        
        // Create a food grouping with static helpings
        var staticHelpings = new List<Helping> { new Helping(staticFood, Servings: 1.5M) };
        var foodGrouping = new FoodGrouping(
            "Rice Bowl",
            staticHelpings,
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
            new MealPrepPlan("XfitDay - Rice Bowl - 2 meals", 
                meal1.Helpings.Select(h => h * 2)), // 2 Xfit days
            new MealPrepPlan("RunningDay - Rice Bowl - 3 meals", 
                meal2.Helpings.Select(h => h * 3)), // 3 Running days
        };
        
        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);
        
        // Act
        var total = weeklyPlan.Total.ToList();
        
        // Assert
        // Find the brown rice in the total
        var riceHelping = total.FirstOrDefault(h => h.Food.Name == "Brown Rice");
        Assert.NotNull(riceHelping);
        
        // Should be: 1.5 servings * (2 Xfit meals + 3 Running meals) = 1.5 * 5 = 7.5
        Assert.Equal(7.5M, riceHelping.Servings);
    }
    
    [Fact]
    public void WeeklyMealsPrepPlan_Total_Should_Combine_Like_Foods_Across_MealPrepPlans()
    {
        // Arrange
        var food1 = new Food("Quinoa",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 120, P: 4.1M, F: 1.9M, CTotal: 21, CFiber: 2.8M));
        var food2 = new Food("Black Beans",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 132, P: 8.9M, F: 0.5M, CTotal: 24, CFiber: 8.7M));
        
        // Create multiple meal prep plans with overlapping foods
        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Plan 1", new List<Helping>
            {
                new Helping(food1, Servings: 2),
                new Helping(food2, Servings: 1.5M)
            }),
            new MealPrepPlan("Plan 2", new List<Helping>
            {
                new Helping(food1, Servings: 3),
                new Helping(food2, Servings: 2.5M)
            }),
            new MealPrepPlan("Plan 3", new List<Helping>
            {
                new Helping(food1, Servings: 1.5M)
                // No food2 in this plan
            })
        };
        
        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);
        
        // Act
        var total = weeklyPlan.Total.OrderBy(h => h.Food.Name).ToList();
        
        // Assert
        Assert.Equal(2, total.Count);
        
        var beansHelping = total.First(h => h.Food.Name == "Black Beans");
        Assert.Equal(4M, beansHelping.Servings); // 1.5 + 2.5
        
        var quinoaHelping = total.First(h => h.Food.Name == "Quinoa");
        Assert.Equal(6.5M, quinoaHelping.Servings); // 2 + 3 + 1.5
    }
    
    [Fact]
    public void CreateMealPrepPlan_Should_Scale_StaticHelpings_By_DaysEatingPreparedMeals()
    {
        // This test uses the actual MuscleGain2 training week
        var trainingWeek = new MuscleGain2();
        var mealPrepPlan = WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek);
        
        // Act
        var total = mealPrepPlan.Total.ToList();
        
        // Assert
        // The total should contain all foods aggregated across all meal prep plans
        Assert.NotEmpty(total);
        
        // Verify that the total is the sum of all helpings from all meal prep plans
        var allHelpingsFromPlans = mealPrepPlan.MealPrepPlans
            .SelectMany(plan => plan.Helpings)
            .ToList();
        
        // Group by food and sum servings
        var expectedTotals = allHelpingsFromPlans
            .GroupBy(h => h.Food)
            .Select(g => new Helping(g.Key, g.Sum(h => h.Servings)))
            .OrderBy(h => h.Food.Name)
            .ToList();
        
        var actualTotals = total.OrderBy(h => h.Food.Name).ToList();
        
        Assert.Equal(expectedTotals.Count, actualTotals.Count);
        
        for (int i = 0; i < expectedTotals.Count; i++)
        {
            Assert.Equal(expectedTotals[i].Food.Name, actualTotals[i].Food.Name);
            Assert.Equal(expectedTotals[i].Servings, actualTotals[i].Servings);
        }
    }
    
    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Include_Totals()
    {
        // Arrange
        var food = new Food("Test Food",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 10, F: 5, CTotal: 15, CFiber: 2));
        
        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Test Plan", new List<Helping> { new Helping(food, Servings: 5) })
        };
        
        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);
        
        // Act
        var output = weeklyPlan.ToString();
        
        // Assert
        Assert.Contains("Test Plan: 500 grams Test Food", output);
        Assert.Contains("Totals:", output);
        Assert.Contains("500 grams Test Food", output);
    }
}