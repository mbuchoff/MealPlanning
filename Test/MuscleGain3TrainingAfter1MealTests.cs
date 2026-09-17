using SystemOfEquations;
using SystemOfEquations.Data.TrainingWeeks.MuscleGain3;

namespace Test;

public class MuscleGain3TrainingAfter1MealTests
{
    [Fact]
    public void NonworkoutMeals_DoNotContainEnglishMuffins()
    {
        var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
            .ForTargetCalories(3000M);

        var nonworkoutServings = trainingWeek.NonworkoutDay.Meals.SelectMany(meal => meal.Servings);

        Assert.DoesNotContain(nonworkoutServings,
            serving => serving.Name.Contains("english muffin", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ForTargetCalories_ReachesRequestedWeeklyAverage()
    {
        const decimal targetDailyCalories = 2800M;
        var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
            .ForTargetCalories(targetDailyCalories);

        var weeklyCalories = trainingWeek.TrainingDays.Sum(
            day => day.ActualNutrients.Cals * day.TrainingDayType.DaysTraining.Count);
        var averageDailyCalories = weeklyCalories / 7;

        Assert.InRange(averageDailyCalories, targetDailyCalories - 0.1M, targetDailyCalories + 0.1M);
    }

    [Fact]
    public void ForTargetCalories_ExplainsTheLimitingFoodGroupingWhenTargetIsBelowSupportedRange()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
                .ForTargetCalories(2000M));

        AssertExplainsLimitingCalculation(exception);
    }

    [Fact]
    public void ForTargetCalories_CanCalculateRunningDayNutrients()
    {
        var exception = Record.Exception(() =>
        {
            var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
                .ForTargetCalories(3000M);
            _ = trainingWeek.RunningDay.ActualNutrients;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ForTargetCalories_ExplainsLimitingFoodGroupingWhenTargetIsAboveSupportedRange()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
                .ForTargetCalories(6000M));

        AssertExplainsLimitingCalculation(exception);
    }

    [Fact]
    public void ToastAndAlmondButter_FallsBackToFourPinnedSlicesWhenEdamameWouldGoNegative()
    {
        // These targets solve "toast and almond butter" to a negative amount of edamame, which
        // used to fail the whole week because every fallback in the chain still used edamame.
        var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 175M)
            .ForTargetCalories(3200M);

        var meal = Assert.Single(
            trainingWeek.XFitDay.Meals,
            m => m.FoodGrouping.Name == "toast and almond butter");

        var toast = Assert.Single(
            meal.Servings,
            serving => serving.Name.Contains("Ezekial Bread", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(4M, toast.NutritionalInformation.ServingUnits);
        Assert.Contains(meal.Servings,
            serving => serving.Name.Contains("almond butter", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(meal.Servings,
            serving => serving.Name.Contains("edamame", StringComparison.OrdinalIgnoreCase));
    }

    // The explanation has to name a specific day, meal and food grouping so the configuration can
    // be fixed. Which grouping is limiting changes as fallbacks are added, so assert the shape of
    // the explanation rather than whichever foods happen to be the bottleneck today.
    private static void AssertExplainsLimitingCalculation(InvalidOperationException exception)
    {
        Assert.Contains("Limiting calculation:", exception.Message);

        var limitingCalculation =
            Assert.IsType<FoodGroupingCalculationException>(exception.InnerException);
        Assert.Contains(limitingCalculation.Message, exception.Message);
        Assert.True(
            limitingCalculation.Message.Split(" > ").Length >= 3,
            $"Expected 'day > meal > food grouping > reason', but got '{limitingCalculation.Message}'.");
    }
}
