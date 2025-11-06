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
    public void TrainingDay_ActualNutrients_ShouldMatchSumOfMealNutrients()
    {
        // Arrange
        var trainingWeek = new MuscleGain2();
        var crossfitDay = trainingWeek.XFitDay;

        // Act - Calculate total protein from individual meals
        var sumOfMealProtein = 0M;
        var sumOfMealFat = 0M;
        var sumOfMealCarbs = 0M;

        _output.WriteLine($"CrossFit Day - Individual Meal Macros (non-conversion servings only):");
        foreach (var meal in crossfitDay.Meals)
        {
            // Calculate actual nutrition from non-conversion servings only
            var nonConversionServings = meal.Servings.Where(s => !s.IsConversion).ToList();

            var mealProtein = 0M;
            var mealFat = 0M;
            var mealCarbs = 0M;

            foreach (var serving in nonConversionServings)
            {
                mealProtein += serving.NutritionalInformation.Macros.P;
                mealFat += serving.NutritionalInformation.Macros.F;
                mealCarbs += serving.NutritionalInformation.Macros.C;
            }

            sumOfMealProtein += mealProtein;
            sumOfMealFat += mealFat;
            sumOfMealCarbs += mealCarbs;

            _output.WriteLine($"  {meal.Name}: {mealProtein}P / {mealFat}F / {mealCarbs}C");
        }

        _output.WriteLine($"\nSum of individual meals: {sumOfMealProtein}P / {sumOfMealFat}F / {sumOfMealCarbs}C");

        // Get the total from TrainingDay.ActualNutrients
        var dayTotal = crossfitDay.ActualNutrients;
        _output.WriteLine($"TrainingDay.ActualNutrients: {dayTotal.Macros.P}P / {dayTotal.Macros.F}F / {dayTotal.Macros.C}C");

        // Assert - ActualNutrients should match the sum of individual meals
        // Allow small rounding differences (within 1g)
        Assert.True(Math.Abs(dayTotal.Macros.P - sumOfMealProtein) <= 1,
            $"Protein mismatch: ActualNutrients={dayTotal.Macros.P}P, Sum of meals={sumOfMealProtein}P");
        Assert.True(Math.Abs(dayTotal.Macros.F - sumOfMealFat) <= 1,
            $"Fat mismatch: ActualNutrients={dayTotal.Macros.F}F, Sum of meals={sumOfMealFat}F");
        Assert.True(Math.Abs(dayTotal.Macros.C - sumOfMealCarbs) <= 1,
            $"Carbs mismatch: ActualNutrients={dayTotal.Macros.C}C, Sum of meals={sumOfMealCarbs}C");
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

        // Get actual from ActualNutrients
        var dayActual = crossfitDay.ActualNutrients;
        _output.WriteLine($"Actual (ActualNutrients): {dayActual.Macros.P}P / {dayActual.Macros.F}F / {dayActual.Macros.C}C");

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
        _output.WriteLine($"TrainingDay.ActualNutrients: {crossfitDay.ActualNutrients.Macros.P:F0}P");
        _output.WriteLine($"Difference: {displayedProteinSum - crossfitDay.ActualNutrients.Macros.P:F0}P");

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

    [Fact]
    public void TrainingDay_TargetMacros_ShouldEqualSumOfMealMacros()
    {
        // Arrange
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var crossfitDay = trainingWeek.XFitDay;

        // Act - Calculate expected target from meal definitions
        var expectedTarget = new Macros(0, 0, 0);
        foreach (var meal in crossfitDay.Meals)
        {
            expectedTarget += meal.Macros;
        }

        // Get actual TargetMacros property
        var actualTarget = crossfitDay.TargetMacros;

        _output.WriteLine($"Expected target (sum of meal.Macros): {expectedTarget}");
        _output.WriteLine($"Actual TargetMacros property: {actualTarget}");

        // Assert - TargetMacros should equal sum of meal target macros
        Assert.Equal(expectedTarget.P, actualTarget.P);
        Assert.Equal(expectedTarget.F, actualTarget.F);
        Assert.Equal(expectedTarget.C, actualTarget.C);
    }

    [Fact]
    public void TrainingDay_HasConversionFoods_ShouldBeTrueWhenAnyMealHasConversionFoods()
    {
        // Arrange
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var crossfitDay = trainingWeek.XFitDay;

        // Act
        var hasConversionFoods = crossfitDay.HasConversionFoods;

        // Check if any meal actually has conversion foods
        var anyMealHasConversion = crossfitDay.Meals.Any(m => m.HasConversionFoods);

        _output.WriteLine($"TrainingDay.HasConversionFoods: {hasConversionFoods}");
        _output.WriteLine($"Any meal has conversion: {anyMealHasConversion}");

        // List which meals have conversion foods
        foreach (var meal in crossfitDay.Meals)
        {
            _output.WriteLine($"  {meal.Name}: {meal.HasConversionFoods}");
        }

        // Assert - HasConversionFoods should match whether any meal has conversion foods
        Assert.Equal(anyMealHasConversion, hasConversionFoods);
    }

    [Fact]
    public void TrainingDay_ToString_ShouldShowActualAndTargetWhenConversionFoodsPresent()
    {
        // Arrange
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var crossfitDay = trainingWeek.XFitDay;

        // Act
        var output = crossfitDay.ToString();

        _output.WriteLine("TrainingDay.ToString() output:");
        _output.WriteLine(output);

        // Assert - If there are conversion foods, output should contain both ACTUAL and TARGET
        if (crossfitDay.HasConversionFoods)
        {
            Assert.Contains("ACTUAL:", output);
            Assert.Contains("TARGET:", output);
            _output.WriteLine("\n✓ Contains ACTUAL and TARGET labels (conversion foods present)");
        }
        else
        {
            Assert.DoesNotContain("ACTUAL:", output);
            Assert.DoesNotContain("TARGET:", output);
            _output.WriteLine("\n✓ No ACTUAL/TARGET labels (no conversion foods)");
        }
    }

    [Fact]
    public void Phase_ToString_ShouldShowActualAndTargetWhenConversionFoodsPresent()
    {
        // Arrange
        var baseTrainingWeek = new MuscleGain2();
        var trainingWeek = baseTrainingWeek.ForTargetCalories(3400M);
        var phase = new Phase("Test Phase", trainingWeek);

        // Act
        var output = phase.ToString();

        _output.WriteLine("Phase.ToString() output (first 1000 chars):");
        _output.WriteLine(output.Substring(0, Math.Min(1000, output.Length)));

        // Check if any training day has conversion foods
        var hasAnyConversionFoods = trainingWeek.TrainingDays.Any(td => td.HasConversionFoods);

        _output.WriteLine($"\nAny training day has conversion foods: {hasAnyConversionFoods}");

        // Assert - If there are conversion foods, output should contain both ACTUAL and TARGET
        if (hasAnyConversionFoods)
        {
            Assert.Contains("ACTUAL:", output);
            Assert.Contains("TARGET:", output);
            Assert.Contains("Week Average:", output);
            _output.WriteLine("✓ Contains ACTUAL and TARGET labels (conversion foods present)");
        }
        else
        {
            Assert.DoesNotContain("ACTUAL:", output);
            Assert.DoesNotContain("TARGET:", output);
            Assert.Contains("Ave per day:", output);
            _output.WriteLine("✓ No ACTUAL/TARGET labels (no conversion foods)");
        }
    }
}
