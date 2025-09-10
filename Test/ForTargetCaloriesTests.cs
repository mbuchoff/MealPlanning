using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;

namespace Test;

public class ForTargetCaloriesTests
{
    // Test-specific TrainingWeek implementation with predictable values
    private record TestTrainingWeek : TrainingWeekBase
    {
        public TestTrainingWeek() : base(
            "Test Training Week",
            nonworkoutMeals: CreateTestMeals(30, 15, 50), // P, F, C per meal (5 meals)
            runningMeals: CreateTestMeals(28, 15, 55),    // 6 meals (including workout shake)
            xfitMeals: CreateTestMeals(28, 15, 65))       // 6 meals (including workout shake)
        {
        }
        
        private static IEnumerable<Meal> CreateTestMeals(decimal proteinPerMeal, decimal fatPerMeal, decimal carbsPerMeal)
        {
            var meals = new List<Meal>();
            
            // Create simple test foods
            var proteinFood = new FoodServing("Test Protein",
                new NutritionalInformation(1, ServingUnits.Gram, Cals: 4, P: 1, F: 0, CTotal: 0, CFiber: 0));
            var fatFood = new FoodServing("Test Fat",
                new NutritionalInformation(1, ServingUnits.Gram, Cals: 9, P: 0, F: 1, CTotal: 0, CFiber: 0));
            var carbFood = new FoodServing("Test Carb",
                new NutritionalInformation(1, ServingUnits.Gram, Cals: 4, P: 0, F: 0, CTotal: 1, CFiber: 0));
            
            var foodGrouping = new FoodGrouping(
                "Test Food Group",
                proteinFood,
                fatFood,
                carbFood,
                FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);
            
            // Create meals with specified macros
            var numMeals = proteinPerMeal == 30 ? 5 : 6; // 5 for non-workout, 6 for workout days
            for (int i = 0; i < numMeals; i++)
            {
                meals.Add(new Meal($"Test Meal {i + 1}", 
                    new Macros(P: proteinPerMeal, F: fatPerMeal, C: carbsPerMeal), 
                    foodGrouping));
            }
            
            return meals;
        }
    }
    [Theory]
    [InlineData(2450, 10)]  // ~90% - near lower bound  
    [InlineData(2500, 5)]   // ~92%
    [InlineData(2650, 5)]   // ~98%
    [InlineData(2900, 5)]   // ~107%
    [InlineData(3100, 10)]  // ~115% - near upper bound
    public void ForTargetCalories_Should_Work_For_Various_Targets(decimal targetCalories, decimal tolerance)
    {
        // Arrange
        var baseTrainingWeek = new TestTrainingWeek();
        
        // Act
        var adjustedWeek = baseTrainingWeek.ForTargetCalories(targetCalories);
        
        // Calculate actual average calories
        var totalCals = 0.0M;
        foreach (var day in adjustedWeek.TrainingDays)
        {
            totalCals += day.TotalNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
        }
        var actualAverage = totalCals / 7;
        
        // Assert - tolerance varies based on how extreme the target is
        Assert.InRange(actualAverage, targetCalories - tolerance, targetCalories + tolerance);
    }
    
    [Fact]
    public void ForTargetCalories_Should_Keep_Protein_Constant()
    {
        // Arrange
        var baseTrainingWeek = new TestTrainingWeek();
        
        // Get base protein
        var baseProtein = 0.0M;
        foreach (var day in baseTrainingWeek.TrainingDays)
        {
            baseProtein += day.TotalNutrients.Macros.P * day.TrainingDayType.DaysTraining.Count;
        }
        baseProtein /= 7;
        
        // Act - adjust to different calorie targets (within valid bounds)
        var lowerCalWeek = baseTrainingWeek.ForTargetCalories(2400M);
        var higherCalWeek = baseTrainingWeek.ForTargetCalories(2900M);
        
        // Calculate protein for each
        var lowerProtein = 0.0M;
        foreach (var day in lowerCalWeek.TrainingDays)
        {
            lowerProtein += day.TotalNutrients.Macros.P * day.TrainingDayType.DaysTraining.Count;
        }
        lowerProtein /= 7;
        
        var higherProtein = 0.0M;
        foreach (var day in higherCalWeek.TrainingDays)
        {
            higherProtein += day.TotalNutrients.Macros.P * day.TrainingDayType.DaysTraining.Count;
        }
        higherProtein /= 7;
        
        // Assert - protein should remain constant (within rounding tolerance)
        Assert.InRange(lowerProtein, baseProtein - 1, baseProtein + 1);
        Assert.InRange(higherProtein, baseProtein - 1, baseProtein + 1);
    }
    
    [Fact]
    public void PlusPercent_And_ForTargetCalories_Should_Both_Work()
    {
        // Arrange
        var baseTrainingWeek = new TestTrainingWeek();
        
        // Act
        // Test that both methods can achieve reasonable calorie targets
        var manualAdjustment = baseTrainingWeek.PlusPercent(95M); // 95% of fats/carbs
        var autoAdjustment = baseTrainingWeek.ForTargetCalories(2600M); // Target 2600 calories
        
        // Calculate calories for each method
        var manualCals = 0.0M;
        foreach (var day in manualAdjustment.TrainingDays)
        {
            manualCals += day.TotalNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
        }
        manualCals /= 7;
        
        var autoCals = 0.0M;
        foreach (var day in autoAdjustment.TrainingDays)
        {
            autoCals += day.TotalNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
        }
        autoCals /= 7;
        
        // Assert - ForTargetCalories should achieve its target accurately
        Assert.InRange(autoCals, 2595, 2605); // Within 5 calories of target
        
        // PlusPercent should reduce calories as expected (95% of fats/carbs)
        Assert.True(manualCals < 2700); // Should be less than base
        Assert.True(manualCals > 2500); // But not too low
    }
    
    [Fact]
    public void ForTargetCalories_Should_Scale_Fats_And_Carbs_Proportionally()
    {
        // Arrange
        var baseTrainingWeek = new TestTrainingWeek();
        
        // Get base fats and carbs ratio
        var baseFats = 0.0M;
        var baseCarbs = 0.0M;
        foreach (var day in baseTrainingWeek.TrainingDays)
        {
            baseFats += day.TotalNutrients.Macros.F * day.TrainingDayType.DaysTraining.Count;
            baseCarbs += day.TotalNutrients.Macros.C * day.TrainingDayType.DaysTraining.Count;
        }
        var baseRatio = baseFats / baseCarbs;
        
        // Act - adjust to different target
        var adjustedWeek = baseTrainingWeek.ForTargetCalories(2500M);
        
        // Calculate adjusted fats and carbs ratio
        var adjustedFats = 0.0M;
        var adjustedCarbs = 0.0M;
        foreach (var day in adjustedWeek.TrainingDays)
        {
            adjustedFats += day.TotalNutrients.Macros.F * day.TrainingDayType.DaysTraining.Count;
            adjustedCarbs += day.TotalNutrients.Macros.C * day.TrainingDayType.DaysTraining.Count;
        }
        var adjustedRatio = adjustedFats / adjustedCarbs;
        
        // Assert - ratio should be preserved (within small tolerance for rounding)
        Assert.InRange(adjustedRatio, baseRatio * 0.98M, baseRatio * 1.02M);
    }
}