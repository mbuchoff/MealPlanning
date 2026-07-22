using SystemOfEquations.Data.TrainingWeeks.MuscleGain3;

namespace Test;

public class MuscleGain3TrainingAfter1MealTests
{
    [Fact]
    public void ForTargetCalories_ReachesRequestedWeeklyAverage()
    {
        const decimal targetDailyCalories = 2800M;
        var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
            .ForTargetCalories(targetDailyCalories);

        var weeklyCalories = trainingWeek.TrainingDays.Sum(
            day => day.ActualNutrients.Cals * day.TrainingDayType.DaysTraining.Count);
        var averageDailyCalories = weeklyCalories / 7;

        Assert.InRange(averageDailyCalories, targetDailyCalories - 1M, targetDailyCalories + 1M);
    }

    [Fact]
    public void ForTargetCalories_ExplainsTheLimitingFoodGroupingWhenTargetCannotBeReached()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M)
                .ForTargetCalories(2000M));

        Assert.Contains("Limiting calculation:", exception.Message);
        Assert.Contains("Non-weight training day", exception.Message);
        Assert.Contains("Waking", exception.Message);
        Assert.Contains("Blueberry oatmeal shake", exception.Message);
        Assert.Contains("oats", exception.Message);
        Assert.NotNull(exception.InnerException);
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
}
