
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

    protected const int MEALS_PER_DAY = 5;

    // +1 to include workout shake
    protected static decimal MuscleGainProteinPerMealOnWorkoutDay(decimal targetGramsProteinPerDay) =>
        targetGramsProteinPerDay / (MEALS_PER_DAY + 1);

    protected static decimal MuscleGainProteinPerMealOnNonworkoutDay(decimal targetGramsProteinPerDay) =>
        targetGramsProteinPerDay / MEALS_PER_DAY;

    protected static decimal FatLossProteinPerMealOnWorkoutDay(decimal targetGramsProteinPerDay) =>
        targetGramsProteinPerDay * 1.25M / (MEALS_PER_DAY + 1);

    protected static decimal FatLossProteinPerMealOnNonworkoutDay(decimal targetGramsProteinPerDay) =>
        targetGramsProteinPerDay * 1.25M / MEALS_PER_DAY;

    internal TrainingWeek PlusPercent(decimal percent) => CloneWithTweakedMacros(
        pMultiplier: 1,
        fMultiplier: percent / 100,
        cMultiplier: percent / 100);

    internal TrainingWeek ForTargetCalories(decimal targetDailyCalories)
    {
        // First calculate the base average calories
        var baseTotalCals = 0.0M;
        foreach (var trainingDay in TrainingDays)
        {
            var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
            baseTotalCals += trainingDay.ActualNutrients.Cals * daysPerWeek;
        }
        var baseAverage = baseTotalCals / 7;

        // Calculate the required percentage based on the ratio
        // Since protein stays constant, we need to estimate based on total calories
        var targetRatio = targetDailyCalories / baseAverage;

        // The actual percentage we need is roughly the target ratio
        // But since protein doesn't scale, we need to adjust
        // Start with a good initial guess
        var percentNeeded = targetRatio * 100;

        // Now do a refined binary search around our estimate
        TrainingWeek bestWeek = this;
        var bestDifference = decimal.MaxValue;

        decimal low = percentNeeded - 10;
        decimal high = percentNeeded + 10;
        decimal epsilon = 0.01M;

        // Ensure bounds are still reasonable
        low = Math.Max(low, 85);
        high = Math.Min(high, 120);

        while (high - low > epsilon)
        {
            var mid = (low + high) / 2;
            var testWeek = PlusPercent(mid);

            // Calculate average daily calories for this test week
            var totalCals = 0.0M;
            foreach (var trainingDay in testWeek.TrainingDays)
            {
                var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
                totalCals += trainingDay.ActualNutrients.Cals * daysPerWeek;
            }
            var averageDailyCals = totalCals / 7;

            var difference = Math.Abs(averageDailyCals - targetDailyCalories);
            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestWeek = testWeek;
            }

            // Stop if we're close enough
            if (difference < 0.1M)
            {
                break;
            }

            // Adjust search range
            if (averageDailyCals < targetDailyCalories)
            {
                low = mid;
            }
            else
            {
                high = mid;
            }
        }

        return bestWeek;
    }
}
