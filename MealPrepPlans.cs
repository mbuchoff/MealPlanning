using SystemOfEquations.Constants;

namespace SystemOfEquations;

internal class MealPrepPlans
{
    public static MealPrepPlan Phase1MealPrepPlan
    {
        get
        {
            var nonworkoutDay = TrainingWeeks.MuscleGain1TrainingWeek.NonworkoutDay;
            var runningDay = TrainingWeeks.MuscleGain1TrainingWeek.RunningDay;
            var xfitDay = TrainingWeeks.MuscleGain1TrainingWeek.XFitDay;

            Helping ConsolidateWorkoutDayHelpings(Food food) =>
                MealPrepPlans.ConsolidateHelpings([(runningDay, 2), (xfitDay, 3)], food);

            return new(
            [
                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.BrownRice_45_Grams)),
                ("running", GetHelping(runningDay, 2, Foods.BrownRice_45_Grams)),
                ("crossfit", GetHelping(xfitDay, 3, Foods.BrownRice_45_Grams)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Farro_52_Gram)),
                ("running", GetHelping(runningDay, 2, Foods.Farro_52_Gram)),
                ("crossfit", GetHelping(xfitDay, 3, Foods.Farro_52_Gram)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x)),
                (null, ConsolidateWorkoutDayHelpings(Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.OliveOil_1_Tbsp)),
                (null, ConsolidateWorkoutDayHelpings(Foods.OliveOil_1_Tbsp)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Tofu_1_5_Block)),
                (null, ConsolidateWorkoutDayHelpings(Foods.Tofu_1_5_Block)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.PumpkinSeeds_30_Grams)),
                (null, ConsolidateWorkoutDayHelpings(Foods.PumpkinSeeds_30_Grams)),
            ]);
        }
    }

    public static MealPrepPlan Phase2MealPrepPlan => new(
        new[]
        {
            new { Multiplier = 3, TrainingType = TrainingDayTypes.XfitDay },
            new { Multiplier = 2, TrainingType = TrainingDayTypes.RunningDay },
        }.Select(x => new
        {
            Day = TrainingWeeks.MuscleGain2TrainingWeek.TrainingDays.Single(td =>
                td.TrainingDayType == x.TrainingType),
            x.Multiplier,
        }).SelectMany(x =>
        new[]
        {
            Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x,
            Foods.BrownRice_45_Grams,
            Foods.OliveOil_1_Tbsp,
            Foods.Tofu_1_5_Block,
            Foods.Farro_52_Gram,
            Foods.PumpkinSeeds_30_Grams,
        }.Select(food => (x.Day, x.Multiplier, Food: food))).Select(x => new
        {
            TrainingDayType = x.Day.TrainingDayType.ToString(),
            Helping = ConsolidateHelpings([(x.Day, x.Multiplier)], x.Food),
        }).Select(x => ((string?) x.TrainingDayType, x.Helping)));

    private static Helping GetHelping(TrainingDay trainingDay, double multiplier, Food food) => new(
        food,
        trainingDay.Meals.SelectMany(m => m.Helpings).Where(h => h.Food.Equals(food))
            .Sum(h => h.Servings) * multiplier);

    private static Helping ConsolidateHelpings(
        IEnumerable<(TrainingDay TrainingDay, double Multiplier)> trainingDays,
        Food food) => new(
            food,
            trainingDays
                .SelectMany(t => t.TrainingDay.Meals.Select(m => (Meal: m, t.Multiplier)))
                .SelectMany(m => m.Meal.Helpings.Select(h => (Helping: h, m.Multiplier)))
                .Where(h => h.Helping.Food.Equals(food))
                .Sum(h => h.Helping.Servings * h.Multiplier));
}
