namespace SystemOfEquations.Constants;

public static class TrainingDayTypes
{
    internal static TrainingDayType NonweightTrainingDay = new(
        "Non-weight training day",
        DaysPerWeek: 2,
        MealPrepsPerWeek: 1);
    internal static TrainingDayType XfitDay = new("Crossfit day", DaysPerWeek: 3, MealPrepsPerWeek: 3);
    internal static TrainingDayType RunningDay = new("Running day", DaysPerWeek: 2, MealPrepsPerWeek: 2);
}
