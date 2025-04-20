using SystemOfEquations.Data.TrainingWeeks;

namespace SystemOfEquations.Data;

internal static class Phases
{
    private static readonly TrainingWeekBase muscleGain1TrainingWeek = new MuscleGain1();
    private static readonly TrainingWeekBase muscleGain2TrainingWeek = new MuscleGain2();
    private static readonly TrainingWeekBase muscleGain3TrainingWeek = new MuscleGain3();
    private static readonly TrainingWeekBase baseTrainingWeek = new Base();

    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1", WeeklyMealsPrepPlans.CreateMealPrepPlan(muscleGain1TrainingWeek), muscleGain1TrainingWeek);
    public static readonly Phase MuscleGain2 = new(
        "Muscle Gain 2", WeeklyMealsPrepPlans.Phase2MealPrepPlan, muscleGain2TrainingWeek);
    public static readonly Phase MuscleGain3 = new(
        "Muscle Gain 3", WeeklyMealsPrepPlans.Phase3MealPrepPlan, muscleGain3TrainingWeek);

    public static Phase MuscleGain3PlusPercent(decimal percent)
    {
        var trainingWeek = muscleGain3TrainingWeek.PlusPercent(100 + percent);
        return new($"Muscle Gain 3, plus {percent} percent",
            WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek);
    }

    public static Phase BasePlusPercent(decimal percent)
    {
        var name = $"Base, plus {percent} percent";
        try
        {
            var trainingWeek = baseTrainingWeek.PlusPercent(100 + percent);
            return new(name, WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek);
        }
        catch (Exception ex)
        {
            throw new Exception($"{name} > {ex.Message}");
        }
    }
}
