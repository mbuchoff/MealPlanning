namespace SystemOfEquations.Data;

public static class TrainingDayTypes
{
    internal static TrainingDayType NonweightTrainingDay = new("Non-weight training day",
        daysTraining: [Day.Saturday, Day.Sunday],
        daysEatingPreparedMeals: 1);
    internal static TrainingDayType XfitDay = new("Crossfit day",
        daysTraining: [Day.Monday, Day.Wednesday, Day.Friday]);
    internal static TrainingDayType RunningDay = new("Running day", daysTraining: [Day.Tuesday, Day.Thursday]);
}
