
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
        TrainingWeek bestWeek = this;
        var baseAverage = CalculateAverageDailyCalories(this);
        var bestDifference = Math.Abs(baseAverage - targetDailyCalories);
        const decimal epsilon = 0.01M;
        const decimal acceptableCalorieDifference = 0.1M;
        var bestPercent = 100M;
        List<(decimal Percent, FoodGroupingCalculationException Exception)> failedCandidates = [];

        if (bestDifference <= acceptableCalorieDifference)
        {
            return bestWeek;
        }

        var targetRatio = targetDailyCalories / baseAverage;
        var percentNeeded = targetRatio * 100;

        decimal low;
        decimal high;

        if (targetDailyCalories > baseAverage)
        {
            low = 100M;
            high = Math.Max(percentNeeded + 10M, 110M);

            while (TryCalculateCandidate(high, out var highWeek, out var highAverageDailyCals))
            {
                UpdateBest(high, highWeek, highAverageDailyCals);
                if (highAverageDailyCals >= targetDailyCalories)
                {
                    break;
                }

                var step = high - low;
                low = high;
                high += Math.Max(step * 2M, 10M);
            }
        }
        else
        {
            high = 100M;
            low = Math.Max(0M, Math.Min(percentNeeded - 10M, 90M));

            while (TryCalculateCandidate(low, out var lowWeek, out var lowAverageDailyCals))
            {
                UpdateBest(low, lowWeek, lowAverageDailyCals);
                if (lowAverageDailyCals <= targetDailyCalories || low == 0M)
                {
                    break;
                }

                var step = high - low;
                high = low;
                low = Math.Max(0M, low - Math.Max(step * 2M, 10M));
            }
        }

        while (high - low > epsilon)
        {
            var mid = (low + high) / 2;
            if (!TryCalculateCandidate(mid, out var testWeek, out var averageDailyCals))
            {
                if (mid > 100M)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
                continue;
            }

            UpdateBest(mid, testWeek, averageDailyCals);

            // Stop if we're close enough
            if (bestDifference <= acceptableCalorieDifference)
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

        if (bestDifference > acceptableCalorieDifference)
        {
            var bestAverageDailyCalories = CalculateAverageDailyCalories(bestWeek);
            var message =
                $"Unable to get within {acceptableCalorieDifference:F1} calories/day of the target average of " +
                $"{targetDailyCalories:F0} calories/day with the configured " +
                $"food groupings. The closest achievable average is {bestAverageDailyCalories:F0} calories/day. " +
                "Add or adjust meal fallback food groupings to support this target.";
            FoodGroupingCalculationException? limitingException = null;
            if (failedCandidates.Count > 0)
            {
                limitingException = failedCandidates
                    .MinBy(failure => Math.Abs(failure.Percent - bestPercent))
                    .Exception;
            }

            if (limitingException != null)
            {
                message += $" Limiting calculation: {limitingException.Message}";
            }

            throw new InvalidOperationException(message, limitingException);
        }

        return bestWeek;

        void UpdateBest(decimal percent, TrainingWeek week, decimal averageDailyCals)
        {
            var difference = Math.Abs(averageDailyCals - targetDailyCalories);
            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestPercent = percent;
                bestWeek = week;
            }
        }

        bool TryCalculateCandidate(
            decimal percent,
            out TrainingWeek trainingWeek,
            out decimal averageDailyCals)
        {
            var success = TryCalculateAverageDailyCalories(
                percent,
                out trainingWeek,
                out averageDailyCals,
                out var calculationException);
            if (!success && calculationException != null)
            {
                failedCandidates.Add((percent, calculationException));
            }

            return success;
        }
    }

    private bool TryCalculateAverageDailyCalories(
        decimal percent,
        out TrainingWeek trainingWeek,
        out decimal averageDailyCals,
        out FoodGroupingCalculationException? calculationException)
    {
        trainingWeek = PlusPercent(percent);
        calculationException = null;
        try
        {
            averageDailyCals = CalculateAverageDailyCalories(trainingWeek);
            return true;
        }
        catch (FoodGroupingCalculationException exception)
        {
            // Some percentages cannot be solved because every fallback produces invalid servings.
            averageDailyCals = 0M;
            calculationException = exception;
            return false;
        }
    }

    private static decimal CalculateAverageDailyCalories(TrainingWeek trainingWeek)
    {
        var totalCals = 0.0M;
        foreach (var trainingDay in trainingWeek.TrainingDays)
        {
            var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
            totalCals += trainingDay.ActualNutrients.Cals * daysPerWeek;
        }

        return totalCals / 7;
    }
}
