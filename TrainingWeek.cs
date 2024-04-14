using SystemOfEquations.Constants;

namespace SystemOfEquations;

internal record TrainingWeek
{
    public TrainingWeek(
        IEnumerable<Meal> nonworkoutMeals,
        IEnumerable<Meal> runningMeals,
        IEnumerable<Meal> xfitMeals)
    {
        NonworkoutMeals = nonworkoutMeals;
        RunningMeals = runningMeals;
        XfitMeals = xfitMeals;

        NonworkoutDay = new(TrainingDayTypes.NonweightTrainingDay, nonworkoutMeals);
        RunningDay = new(TrainingDayTypes.RunningDay, runningMeals);
        XFitDay = new(TrainingDayTypes.XfitDay, xfitMeals);
    }

    public IEnumerable<Meal> NonworkoutMeals { get; }
    public IEnumerable<Meal> RunningMeals { get; }
    public IEnumerable<Meal> XfitMeals { get; }

    public TrainingDay NonworkoutDay { get; }
    public TrainingDay RunningDay { get; }
    public TrainingDay XFitDay { get; }

    public IEnumerable<TrainingDay> TrainingDays => [NonworkoutDay, RunningDay, XFitDay];

    public TrainingWeek CloneWithTweakedMacros(double pMultiplier, double fMultiplier, double cMultiplier) => new(
        NonworkoutMeals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        RunningMeals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        XfitMeals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)));
}
