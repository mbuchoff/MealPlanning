using SystemOfEquations.Data;

namespace SystemOfEquations;

internal record TrainingWeek
{
    public TrainingWeek(
        string name,
        IEnumerable<Meal> nonworkoutMeals,
        IEnumerable<Meal> runningMeals,
        IEnumerable<Meal> xfitMeals)
    {
        Name = name;
        NonworkoutDay = CreateTrainingDay(TrainingDayTypes.NonweightTrainingDay, nonworkoutMeals);
        RunningDay = CreateTrainingDay(TrainingDayTypes.RunningDay, runningMeals);
        XFitDay = CreateTrainingDay(TrainingDayTypes.XfitDay, xfitMeals);
    }

    public string Name { get; }
    public TrainingDay NonworkoutDay { get; }
    public TrainingDay RunningDay { get; }
    public TrainingDay XFitDay { get; }

    public IEnumerable<TrainingDay> TrainingDays => [NonworkoutDay, RunningDay, XFitDay];

    public TrainingWeek CloneWithTweakedMacros(double pMultiplier, double fMultiplier, double cMultiplier) => new(
        Name,
        NonworkoutDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        RunningDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)),
        XFitDay.Meals.Select(m => m.CloneWithTweakedMacros(pMultiplier, fMultiplier, cMultiplier)));

    private TrainingDay CreateTrainingDay(TrainingDayType trainingDayType, IEnumerable<Meal> meals)
    {
        try
        {
            return new(trainingDayType, meals);
        }
        catch (Exception ex)
        {
            throw new Exception($"{Name} > {ex.Message}");
        }
    }
}
