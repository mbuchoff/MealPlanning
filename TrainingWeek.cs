using static SystemOfEquations.TrainingDay;

namespace SystemOfEquations;

internal record TrainingWeek
{
    public TrainingWeek(
        IEnumerable<Meal> nonworkoutMeals,
        IEnumerable<Meal> runningMeals,
        IEnumerable<Meal> xfitMeals)
    {
        this.NonworkoutDay = new(TrainingTypeEnum.NonWeightTrainingDay, nonworkoutMeals);
        this.RunningDay = new(TrainingTypeEnum.RunningDay, runningMeals);
        this.XFitDay = new(TrainingTypeEnum.CrossfitDay, xfitMeals);
    }

    public TrainingDay NonworkoutDay { get; }
    public TrainingDay RunningDay { get; }
    public TrainingDay XFitDay { get; }

    public IEnumerable<TrainingDay> TrainingDays => [NonworkoutDay, RunningDay, XFitDay];
}
