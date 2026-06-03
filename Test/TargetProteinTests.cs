using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Data.TrainingWeeks.MuscleGain3;

namespace Test;

public class TargetProteinTests
{
    [Fact]
    public void MuscleGain2_UsesRequested175GramsProteinPerDay()
    {
        var trainingWeek = new MuscleGain2(175);

        AssertDailyProteinTarget(175, trainingWeek.NonworkoutDay.TargetMacros.P);
        AssertDailyProteinTarget(175, trainingWeek.RunningDay.TargetMacros.P);
        AssertDailyProteinTarget(175, trainingWeek.XFitDay.TargetMacros.P);
    }

    [Fact]
    public void MuscleGain3TrainingAfter1Meal_UsesRequestedProteinTarget()
    {
        var trainingWeek = new MuscleGain3TrainingAfter1Meal(targetGramsProteinPerDay: 212.5M);

        AssertDailyProteinTarget(212.5M, trainingWeek.NonworkoutDay.TargetMacros.P);
        AssertDailyProteinTarget(212.5M, trainingWeek.RunningDay.TargetMacros.P);
        AssertDailyProteinTarget(212.5M, trainingWeek.XFitDay.TargetMacros.P);
    }

    private static void AssertDailyProteinTarget(decimal expected, decimal actual)
    {
        Assert.True(Math.Abs(expected - actual) < 0.0001M, $"Expected {expected}g protein, actual {actual}g.");
    }
}
