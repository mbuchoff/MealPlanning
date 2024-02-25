namespace SystemOfEquations;

public static class Phases
{
    public static readonly Phase MuscleGain1 = new(
        "Muscle Gain 1", MealPrepPlans.Phase1MealPrepPlan, TrainingDays.Phase1TrainingDays);
}
