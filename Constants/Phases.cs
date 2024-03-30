namespace SystemOfEquations.Constants;

internal static class Phases
{
    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1", MealPrepPlans.Phase1MealPrepPlan, TrainingWeeks.MuscleGain1TrainingWeek);
    public static readonly Phase MuscleGain2 = new(
        "Muscle Gain 2", MealPrepPlans.Phase2MealPrepPlan, TrainingWeeks.MuscleGain2TrainingWeek);
    public static readonly Phase MuscleGain3 = new(
        "Muscle Gain 3", MealPrepPlans.Phase3MealPrepPlan, TrainingWeeks.MuscleGain3TrainingWeek);
}
