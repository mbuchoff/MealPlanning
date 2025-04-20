
namespace SystemOfEquations.Data.TrainingWeeks;

internal abstract record TrainingWeekBase : TrainingWeek
{
    protected TrainingWeekBase(
        string name,
        IEnumerable<Meal> nonworkoutMeals,
        IEnumerable<Meal> runningMeals,
        IEnumerable<Meal> xfitMeals) : base(name, nonworkoutMeals, runningMeals, xfitMeals)
    {
    }

    private const decimal TARGET_WEIGHT = 165;
    private const int MEALS_PER_DAY = 5;

    // +1 to include workout shake
    protected const decimal MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY = TARGET_WEIGHT / (MEALS_PER_DAY + 1);
    protected const decimal FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY = TARGET_WEIGHT * 1.25M / (MEALS_PER_DAY + 1);

    protected const decimal MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY = TARGET_WEIGHT / MEALS_PER_DAY;
    protected const decimal FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY = TARGET_WEIGHT * 1.25M / MEALS_PER_DAY;

    internal TrainingWeek PlusPercent(decimal percent) => CloneWithTweakedMacros(
        pMultiplier: 1,
        fMultiplier: percent / 100,
        cMultiplier: percent / 100);
}
