namespace SystemOfEquations.Data;

internal static class Phases
{
    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1",
        WeeklyMealsPrepPlans.CreateMealPrepPlan(TrainingWeeks.MuscleGain1TrainingWeek),
        TrainingWeeks.MuscleGain1TrainingWeek);
    public static readonly Phase MuscleGain2 = new(
        "Muscle Gain 2", WeeklyMealsPrepPlans.Phase2MealPrepPlan, TrainingWeeks.MuscleGain2TrainingWeek);
    public static readonly Phase MuscleGain3 = new(
        "Muscle Gain 3", WeeklyMealsPrepPlans.Phase3MealPrepPlan, TrainingWeeks.MuscleGain3TrainingWeek);

    public static Phase MuscleGain3PlusPercent(decimal percent)
    {
        var trainingWeek = TrainingWeeks.MuscleGain3PlusPercentTrainingWeek(100 + percent);
        return new($"Muscle Gain 3, plus {percent} percent",
            WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek);
    }

    public static Phase BasePlusPercent(decimal percent)
    {
        var name = $"Base, plus {percent} percent";
        try
        {
            var trainingWeek = TrainingWeeks.BasePlusPercentTrainingWeek(100 + percent);
            return new(name, WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek);
        }
        catch (Exception ex)
        {
            throw new Exception($"{name} > {ex.Message}");
        }
    }
}
