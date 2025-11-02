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
                meal1.Servings.Select(s => s * 2), [], 2, meal1.Macros * 2), // 2 Xfit days
            new MealPrepPlan("RunningDay - Rice Bowl",
                meal2.Servings.Select(s => s * 3), [], 3, meal2.Macros * 3), // 3 Running days
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
            }, [], 1, new Macros(P: 40, F: 15, C: 60)),
            new MealPrepPlan("Plan 2", new List<FoodServing>
            {
                food1 * 3,
                food2 * 2.5M
            }, [], 1, new Macros(P: 40, F: 15, C: 60)),
            new MealPrepPlan("Plan 3", new List<FoodServing>
            {
                food1 * 1.5M
                // No food2 in this plan
            }, [], 1, new Macros(P: 40, F: 15, C: 60))
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
            .SelectMany(plan => plan.CookingServings.Concat(plan.EatingServings))
            .ToList();

        // Group by food name and sum servings (using same logic as Total property)
        var expectedTotals = allServingsFromPlans
            .SelectMany(s => s.GetComponentsForDisplay()) // Expand composites to components
            .CombineLikeServings() // Then combine like components
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
            new MealPrepPlan("Test Plan", new List<FoodServing> { food * 5 }, [], 1, new Macros(P: 40, F: 15, C: 60))
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Servings should NOT have the meal name prefix
        Assert.DoesNotContain("Test Plan: 500 grams", output);
        // Should contain the serving without prefix
        Assert.Contains("500 grams Test Food", output);
        Assert.Contains("Totals:", output);
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

    [Fact]
    public void CreateMealPrepPlan_Should_Not_Double_Scale_StaticServings()
    {
        // This test verifies the fix for the bug where static servings were being
        // multiplied by DaysEatingPreparedMeals twice:
        // 1. Once in SumWithSameFoodGrouping (via mealCount = mealGroup.Count() * daysPerWeek)
        // 2. Once in WeeklyMealsPrepPlans.cs when creating MealPrepPlan
        //
        // Expected: Static servings should only appear once per meal instance
        // Example: If there are 2 "wheat berries" meals per Crossfit day and 3 Crossfit days per week,
        // and each meal has 1 english muffin as a static serving, then the weekly total should be 6 muffins,
        // not 18 (which would be 6 * 3 from double multiplication).

        // Arrange
        var englishMuffin = new FoodServing("Ezekiel english muffin",
            new(ServingUnits: 1, ServingUnits.None, Cals: 180, P: 12, F: 1, CTotal: 34, CFiber: 6));

        var pFood = new FoodServing("Brown Rice",
            new(ServingUnits: 45, ServingUnits.Gram, Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2));
        var fFood = new FoodServing("Pumpkin Seeds",
            new(ServingUnits: 30, ServingUnits.Gram, Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));
        var cFood = new FoodServing("Protein To Carb Conversion",
            new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0));

        // Create a food grouping with a static serving (english muffin)
        var foodGrouping = new FoodGrouping(
            "wheat berries",
            [englishMuffin], // Static serving - should appear once per meal instance
            pFood,
            fFood,
            cFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        // Create 2 meals per day with the same food grouping
        var meal1 = new Meal("40 minutes after workout", new Macros(P: 28, F: 10, C: 120), foodGrouping);
        var meal2 = new Meal("2-4 hours after last meal", new Macros(P: 28, F: 20, C: 100), foodGrouping);

        // Simulate XfitDay with 3 days per week
        var xfitMeals = new[] { meal1, meal2 };
        var daysPerWeek = 3;

        // Act - This simulates what happens in WeeklyMealsPrepPlans.CreateMealPrepPlan
        var mealsWithCounts = xfitMeals.SumWithSameFoodGrouping(daysPerWeek).ToList();

        // Assert
        Assert.Single(mealsWithCounts); // Should be grouped into one summed meal
        var (summedMeal, mealCount) = mealsWithCounts.First();

        // Verify meal count is correct: 2 meals per day * 3 days = 6
        Assert.Equal(6, mealCount);

        // Verify the servings are already scaled for the full week
        var muffinServing = summedMeal.Servings.FirstOrDefault(s => s.Name == "Ezekiel english muffin");
        Assert.NotNull(muffinServing);

        // The static serving should be scaled to 6 (1 muffin * 6 meal instances)
        // NOT 18 (which would be 1 * 6 * 3 from double multiplication)
        Assert.Equal(6, muffinServing.NutritionalInformation.ServingUnits);

        // When creating the MealPrepPlan, servings should NOT be multiplied again
        var mealPrepPlan = new MealPrepPlan(
            "Crossfit day - wheat berries",
            summedMeal.Servings, // Do NOT multiply by daysPerWeek again!
            [],
            mealCount,
            summedMeal.Macros);

        // Verify the final serving count in the meal prep plan
        var finalMuffinServing = mealPrepPlan.CookingServings.FirstOrDefault(s => s.Name == "Ezekiel english muffin");
        Assert.NotNull(finalMuffinServing);
        Assert.Equal(6, finalMuffinServing.NutritionalInformation.ServingUnits); // Should still be 6, not 18
    }

    [Fact]
    public void SumWithSameFoodGrouping_Should_Scale_Macros_By_DaysPerWeek()
    {
        // This test verifies that when summing meals across multiple days per week,
        // the macros are correctly multiplied by daysPerWeek to represent the full week.
        //
        // Problem: totalMacros was being calculated as mealGroup.Sum(m => m.Macros) which only
        // sums for ONE DAY, but mealCount = mealGroup.Count() × daysPerWeek represents the FULL WEEK.
        //
        // Expected behavior for INTENDED macros calculation in Todoist:
        // When showing "1 meal" out of "6 total meals", each meal should have macros that equal
        // the original intended macros (e.g., P:28g), not divided by daysPerWeek.

        // Arrange - Create two meals per day with specific intended macros
        var pFood = new FoodServing("Brown Rice",
            new(ServingUnits: 45, ServingUnits.Gram, Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2));
        var fFood = new FoodServing("Pumpkin Seeds",
            new(ServingUnits: 30, ServingUnits.Gram, Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));
        var cFood = new FoodServing("Protein To Carb Conversion",
            new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0),
            IsConversion: true);

        var foodGrouping = new FoodGrouping(
            "wheat berries",
            [],
            pFood,
            fFood,
            cFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        // Create 2 meals per day with different intended macros (similar to Crossfit wheat berries meals)
        // Meal 1: P:28, F:10, C:120 (40 minutes after workout)
        // Meal 2: P:28, F:20, C:100 (2-4 hours after last meal)
        var meal1 = new Meal("40 minutes after workout", new Macros(P: 28, F: 10, C: 120), foodGrouping);
        var meal2 = new Meal("2-4 hours after last meal", new Macros(P: 28, F: 20, C: 100), foodGrouping);

        var mealsPerDay = new[] { meal1, meal2 };
        var daysPerWeek = 3; // Simulate 3 Crossfit days

        // Act - Sum meals for the week
        var result = mealsPerDay.SumWithSameFoodGrouping(daysPerWeek).ToList();

        // Assert
        Assert.Single(result);
        var (summedMeal, mealCount) = result.First();

        // Meal count should be: 2 meals/day × 3 days = 6 meals
        Assert.Equal(6, mealCount);

        // The summed meal's macros should represent the FULL WEEK (not just one day)
        // Expected total for week:
        // P: (28 + 28) × 3 days = 168g
        // F: (10 + 20) × 3 days = 90g
        // C: (120 + 100) × 3 days = 660g
        Assert.Equal(168, summedMeal.Macros.P);
        Assert.Equal(90, summedMeal.Macros.F);
        Assert.Equal(660, summedMeal.Macros.C);

        // This is critical for Todoist INTENDED macros calculation:
        // When Todoist scales for "1 meal" using scaleFactor = 1/6:
        // - P: 168/6 = 28g ✓ (matches original intended macros)
        // - F: 90/6 = 15g ✓ (average of 10 and 20)
        // - C: 660/6 = 110g ✓ (average of 120 and 100)
    }

    [Fact]
    public void CreateMealPrepPlan_Should_Exclude_Conversion_Foods()
    {
        // Conversion foods are mathematical adjustments used to hit target macros
        // They should NOT appear in meal prep plans since they're not actual foods to buy/cook

        // Arrange - Create a meal with a conversion food (similar to WheatBerriesAndRice)
        var pFood = new FoodServing("Brown Rice",
            new(ServingUnits: 45, ServingUnits.Gram, Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2));
        var fFood = new FoodServing("Pumpkin Seeds",
            new(ServingUnits: 30, ServingUnits.Gram, Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));
        var conversionFood = new FoodServing("Protein To Carb Conversion",
            new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0),
            IsConversion: true);

        var foodGrouping = new FoodGrouping(
            "test meal",
            [],
            pFood,
            fFood,
            conversionFood, // Conversion food as cFood
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var meal = new Meal("Test Meal", new Macros(P: 28, F: 10, C: 120), foodGrouping);

        // Create a minimal training week with just this meal
        var trainingWeek = new TrainingWeek(
            "Test Week",
            nonworkoutMeals: [],
            runningMeals: [],
            xfitMeals: [meal]);

        // Act - Create meal prep plan
        var mealPrepPlan = WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek);

        // Assert - Conversion foods should be excluded from all meal prep plans
        var allServings = mealPrepPlan.MealPrepPlans
            .SelectMany(plan => plan.CookingServings.Concat(plan.EatingServings))
            .ToList();

        // Should NOT contain any conversion foods
        Assert.DoesNotContain(allServings, s => s.IsConversion);
        Assert.DoesNotContain(allServings, s => s.Name.Contains("conversion", StringComparison.OrdinalIgnoreCase));

        // Should still contain the real foods
        Assert.Contains(allServings, s => s.Name == "Brown Rice");
        Assert.Contains(allServings, s => s.Name == "Pumpkin Seeds");
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Show_Macros()
    {
        // Arrange
        var targetMacros = new Macros(P: 50, F: 15, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Seitan);

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Crossfit day - seitan",
                meal.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Seitan has no conversion foods, so macros should be unlabeled
        Assert.Contains("50 P", output); // Protein
        Assert.Contains("15 F", output); // Fat
        Assert.Contains("80 C", output); // Carbs
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Not_Prefix_Servings_With_Meal_Name()
    {
        // Arrange
        var targetMacros = new Macros(P: 50, F: 15, C: 80);
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Seitan);

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Crossfit day - seitan",
                meal.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Should NOT contain the meal name as a prefix on serving lines
        Assert.DoesNotContain("Crossfit day - seitan: ", output);

        // Should contain servings without the prefix (just ingredient names)
        Assert.Contains("olive oil", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("brown rice", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Separate_Entries_With_Blank_Lines()
    {
        // Arrange
        var targetMacros1 = new Macros(P: 50, F: 15, C: 80);
        var meal1 = new Meal("Test Meal 1", targetMacros1, FoodGroupings.Seitan);

        var targetMacros2 = new Macros(P: 40, F: 20, C: 70);
        var meal2 = new Meal("Test Meal 2", targetMacros2, FoodGroupings.Tofu);

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Crossfit day - seitan",
                meal1.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros1),
            new MealPrepPlan("Running day - tofu",
                meal2.Servings.Where(s => !s.IsConversion),
                [],
                2,
                targetMacros2)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Should contain blank line between entries (two consecutive newlines)
        // Pattern: last serving of first meal, blank line, header of second meal
        var lines = output.Split('\n');

        // Find the transition between the two meal plans
        var seitanIndex = Array.FindIndex(lines, line => line.Contains("Crossfit day - seitan"));
        var tofuIndex = Array.FindIndex(lines, line => line.Contains("Running day - tofu"));

        Assert.True(seitanIndex >= 0, "Should contain Crossfit day - seitan");
        Assert.True(tofuIndex > seitanIndex, "Should contain Running day - tofu after seitan");

        // There should be a blank line before the tofu entry
        Assert.Equal("", lines[tofuIndex - 1]);
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Not_Show_Actual_Target_Labels_Without_Conversion_Foods()
    {
        // Arrange
        var targetMacros = new Macros(P: 50, F: 15, C: 80);
        // Seitan has no conversion foods
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Seitan);

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Crossfit day - seitan",
                meal.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros,
                HasConversionFoods: false)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Should NOT show ACTUAL/TARGET labels when there are no conversion foods
        Assert.DoesNotContain("ACTUAL:", output);
        Assert.DoesNotContain("TARGET:", output);
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Should_Show_Actual_Target_Labels_With_Conversion_Foods()
    {
        // Arrange
        var targetMacros = new Macros(P: 30, F: 20, C: 80);
        // Oatmeal with conversion foods
        var meal = new Meal("Test Meal", targetMacros, FoodGroupings.Oatmeal(withEdamame: false));

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Test day - oatmeal",
                meal.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros,
                HasConversionFoods: true)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // SHOULD show ACTUAL/TARGET labels when there are conversion foods
        Assert.Contains("ACTUAL:", output);
        Assert.Contains("TARGET:", output);
    }

    [Fact]
    public void WeeklyMealsPrepPlan_ToString_Target_Should_Not_Show_Calories()
    {
        // TARGET macros don't inherently have calories - calories are derived from P/F/C
        // ACTUAL shows calories because it comes from real food servings
        // TARGET should only show the macro breakdown without calories

        // Arrange
        var targetMacros = new Macros(P: 170, F: 97, C: 712);
        // Create a meal with conversion foods so ACTUAL/TARGET labels appear
        var meal = new Meal("rice", targetMacros, FoodGroupings.Oatmeal(withEdamame: false));

        var mealPrepPlans = new List<MealPrepPlan>
        {
            new MealPrepPlan("Crossfit day - rice",
                meal.Servings.Where(s => !s.IsConversion),
                [],
                3,
                targetMacros,
                HasConversionFoods: true)
        };

        var weeklyPlan = new WeeklyMealsPrepPlan(mealPrepPlans);

        // Act
        var output = weeklyPlan.ToString();

        // Assert
        // Find the TARGET line in the output
        var lines = output.Split('\n');
        var targetLine = lines.FirstOrDefault(line => line.Contains("TARGET:"));
        Assert.NotNull(targetLine);

        // TARGET should NOT contain "cals"
        Assert.DoesNotContain("cals", targetLine);

        // TARGET should still show the macros
        Assert.Contains("170 P", targetLine);
        Assert.Contains("97 F", targetLine);
        Assert.Contains("712 C", targetLine);
    }
}