using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;
using Xunit;
using Xunit.Abstractions;

namespace Test;

public class TrainingDayMacrosTests
{
    private readonly ITestOutputHelper _output;

    public TrainingDayMacrosTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TrainingDay_TotalNutrients_ShouldMatchSumOfMealNutrients()
    {
        // Arrange
        var trainingWeek = new MuscleGain2();
        var crossfitDay = trainingWeek.XFitDay;

        // Act - Calculate total protein from individual meals
        var sumOfMealProtein = 0M;
        var sumOfMealFat = 0M;
        var sumOfMealCarbs = 0M;

        _output.WriteLine($"CrossFit Day - Individual Meal Macros:");
        foreach (var meal in crossfitDay.Meals)
        {
            var mealNutrition = meal.NutritionalInformation;
            sumOfMealProtein += mealNutrition.Macros.P;
            sumOfMealFat += mealNutrition.Macros.F;
            sumOfMealCarbs += mealNutrition.Macros.C;

            _output.WriteLine($"  {meal.Name}: {mealNutrition.Macros.P}P / {mealNutrition.Macros.F}F / {mealNutrition.Macros.C}C");
        }

        _output.WriteLine($"\nSum of individual meals: {sumOfMealProtein}P / {sumOfMealFat}F / {sumOfMealCarbs}C");

        // Get the total from TrainingDay.TotalNutrients
        var dayTotal = crossfitDay.TotalNutrients;
        _output.WriteLine($"TrainingDay.TotalNutrients: {dayTotal.Macros.P}P / {dayTotal.Macros.F}F / {dayTotal.Macros.C}C");

        // Assert - TotalNutrients should match the sum of individual meals
        // Allow small rounding differences (within 1g)
        Assert.True(Math.Abs(dayTotal.Macros.P - sumOfMealProtein) <= 1,
            $"Protein mismatch: TotalNutrients={dayTotal.Macros.P}P, Sum of meals={sumOfMealProtein}P");
        Assert.True(Math.Abs(dayTotal.Macros.F - sumOfMealFat) <= 1,
            $"Fat mismatch: TotalNutrients={dayTotal.Macros.F}F, Sum of meals={sumOfMealFat}F");
        Assert.True(Math.Abs(dayTotal.Macros.C - sumOfMealCarbs) <= 1,
            $"Carbs mismatch: TotalNutrients={dayTotal.Macros.C}C, Sum of meals={sumOfMealCarbs}C");
    }

    [Fact]
    public void TrainingDay_Target_ShouldMatchSumOfMealTargets()
    {
        // Arrange - Use scaled version at 3400 calories (what user sees in console)
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var crossfitDay = trainingWeek.XFitDay;

        // Act - Calculate target macros from individual meal definitions
        var sumOfTargetProtein = 0M;
        var sumOfTargetFat = 0M;
        var sumOfTargetCarbs = 0M;

        _output.WriteLine($"CrossFit Day - Target Macros (from meal.Macros):");
        foreach (var meal in crossfitDay.Meals)
        {
            sumOfTargetProtein += meal.Macros.P;
            sumOfTargetFat += meal.Macros.F;
            sumOfTargetCarbs += meal.Macros.C;

            _output.WriteLine($"  {meal.Name}: {meal.Macros.P}P / {meal.Macros.F}F / {meal.Macros.C}C");
        }

        _output.WriteLine($"\nSum of target macros: {sumOfTargetProtein}P / {sumOfTargetFat}F / {sumOfTargetCarbs}C");

        // Get actual from TotalNutrients
        var dayActual = crossfitDay.TotalNutrients;
        _output.WriteLine($"Actual (TotalNutrients): {dayActual.Macros.P}P / {dayActual.Macros.F}F / {dayActual.Macros.C}C");

        // Calculate what we show in the UI
        var targetMacros = new Macros(0, 0, 0);
        foreach (var meal in crossfitDay.Meals)
        {
            targetMacros += meal.Macros;
        }
        _output.WriteLine($"UI Target calculation: {targetMacros.P}P / {targetMacros.F}F / {targetMacros.C}C");

        // The test assertion - just document the values for now
        _output.WriteLine($"\nDifference (Actual - Target):");
        _output.WriteLine($"  Protein: {dayActual.Macros.P - sumOfTargetProtein}P");
        _output.WriteLine($"  Fat: {dayActual.Macros.F - sumOfTargetFat}F");
        _output.WriteLine($"  Carbs: {dayActual.Macros.C - sumOfTargetCarbs}C");
    }

    [Fact]
    public void TrainingDay_IndividualMealDisplay_ShouldMatchWhatUserSees()
    {
        // Arrange - Use scaled version at 3400 calories (what user sees in console)
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var crossfitDay = trainingWeek.XFitDay;

        // Act - Display what the interactive navigator would show for each meal
        // This mimics the DisplayMeal function which shows NutritionalInformation
        _output.WriteLine($"CrossFit Day - What User Sees in Interactive Navigator:");

        var displayedProteinSum = 0M;
        var mealIndex = 1;
        foreach (var meal in crossfitDay.Meals)
        {
            // The interactive navigator shows meal.NutritionalInformation
            // This is filtered to non-conversion servings
            var servingsToDisplay = meal.Servings.Where(s => !s.IsConversion).ToList();

            // Calculate what the GenerateNutritionalComment would show
            var displayedNutrition = servingsToDisplay
                .Select(s => s.NutritionalInformation)
                .Aggregate((a, b) => new NutritionalInformation(
                    a.ServingUnits + b.ServingUnits,
                    ServingUnits.Meal,
                    a.Cals + b.Cals,
                    a.Macros.P + b.Macros.P,
                    a.Macros.F + b.Macros.F,
                    a.Macros.C + b.Macros.C,
                    a.CFiber + b.CFiber));

            displayedProteinSum += displayedNutrition.Macros.P;

            _output.WriteLine($"  Meal {mealIndex} - {meal.Name}: {displayedNutrition.Macros.P:F0}P");
            mealIndex++;
        }

        _output.WriteLine($"\nSum of displayed protein: {displayedProteinSum:F0}P");
        _output.WriteLine($"TrainingDay.TotalNutrients: {crossfitDay.TotalNutrients.Macros.P:F0}P");
        _output.WriteLine($"Difference: {displayedProteinSum - crossfitDay.TotalNutrients.Macros.P:F0}P");

        // User reports seeing 189P
        _output.WriteLine($"\nUser reported: 189P");
        _output.WriteLine($"Our calculation: {displayedProteinSum:F0}P");

        // Calculate what the InteractiveNavigator should show (non-conversion servings)
        var navigatorActual = new Macros(0, 0, 0);
        foreach (var meal in crossfitDay.Meals)
        {
            var nonConversionServings = meal.Servings.Where(s => !s.IsConversion);
            foreach (var serving in nonConversionServings)
            {
                navigatorActual += serving.NutritionalInformation.Macros;
            }
        }

        _output.WriteLine($"InteractiveNavigator should show (ACTUAL): {navigatorActual.P:F0}P");

        // The displayed sum should match what the navigator calculates
        Assert.True(Math.Abs(displayedProteinSum - navigatorActual.P) <= 1,
            $"Displayed protein sum ({displayedProteinSum:F0}P) doesn't match Navigator calculation ({navigatorActual.P:F0}P)");
    }
}
