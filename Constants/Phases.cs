namespace SystemOfEquations.Constants;

internal static class Phases
{
    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1",
        MealPrepPlans.CreateMealPrepPlan(TrainingWeeks.MuscleGain1TrainingWeek),
        TrainingWeeks.MuscleGain1TrainingWeek);
    public static readonly Phase MuscleGain2 = new(
        "Muscle Gain 2", MealPrepPlans.Phase2MealPrepPlan, TrainingWeeks.MuscleGain2TrainingWeek);
    public static readonly Phase MuscleGain3 = new(
        "Muscle Gain 3", MealPrepPlans.Phase3MealPrepPlan, TrainingWeeks.MuscleGain3TrainingWeek);

    public static Phase MuscleGain3PlusPercent(double percent)
    {
        var trainingWeek = TrainingWeeks.MuscleGain3PlusPercentTrainingWeek(100 + percent);
        return new(
            $"Muscle Gain 3, plus {percent} percent", MealPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek);
    }
}
