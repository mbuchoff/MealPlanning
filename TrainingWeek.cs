using SystemOfEquations.Constants;

namespace SystemOfEquations;

internal record TrainingWeek
{
    public TrainingWeek(
        IEnumerable<Meal> nonworkoutMeals,
        IEnumerable<Meal> runningMeals,
        IEnumerable<Meal> xfitMeals)
    {
        NonworkoutDay = new(TrainingDayTypes.NonweightTrainingDay, nonworkoutMeals);
        RunningDay = new(TrainingDayTypes.RunningDay, runningMeals);
        XFitDay = new(TrainingDayTypes.XfitDay, xfitMeals);
    }

    public TrainingDay NonworkoutDay { get; }
    public TrainingDay RunningDay { get; }
    public TrainingDay XFitDay { get; }

    public IEnumerable<TrainingDay> TrainingDays => [NonworkoutDay, RunningDay, XFitDay];

    public TrainingWeek CloneWithTweakedMacros(double pMultiplier, double fMultiplier, double cMultiplier) => new(
        NonworkoutDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        RunningDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        XFitDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)));
}
