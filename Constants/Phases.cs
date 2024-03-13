namespace SystemOfEquations.Constants;

internal static class Phases
{
    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1", MealPrepPlans.Phase1MealPrepPlan, TrainingDays.MuscleGain1TrainingDays);
    public static readonly Phase MuscleGain2 = new(
        "Muscle Gain 2", MealPrepPlans.Phase2MealPrepPlan, TrainingDays.MuscleGain2TrainingDays);
}
