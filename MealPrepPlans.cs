namespace SystemOfEquations;

internal class MealPrepPlans
{
    public static MealPrepPlan Phase1MealPrepPlan
    {
        get
        {
            var nonworkoutDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Non-weight training day");
            var runningDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Running day");
            var xfitDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Crossfit day");

            Helping ConsolidateWorkoutDayHelpings(Food food) =>
                MealPrepPlans.ConsolidateHelpings([(runningDay, 2), (xfitDay, 3)], food);

            return new(
            [
                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.BrownRice_1_Cup)),
                ("running", GetHelping(runningDay, 2, Foods.BrownRice_1_Cup)),
                ("crossfit", GetHelping(xfitDay, 3, Foods.BrownRice_1_Cup)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Farro_1_Cup)),
                ("running", GetHelping(runningDay, 2, Foods.Farro_1_Cup)),
                ("crossfit", GetHelping(xfitDay, 3, Foods.Farro_1_Cup)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Seitan_Yeast_1_Cup_Gluten_2x)),
                (null, ConsolidateWorkoutDayHelpings(Foods.Seitan_Yeast_1_Cup_Gluten_2x)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.OliveOil_1_Tbsp)),
                (null, ConsolidateWorkoutDayHelpings(Foods.OliveOil_1_Tbsp)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.Tofu_1_5_block)),
                (null, ConsolidateWorkoutDayHelpings(Foods.Tofu_1_5_block)),

                ("nonworkout", GetHelping(nonworkoutDay, 1, Foods.PumpkinSeeds_1_Cup)),
                (null, ConsolidateWorkoutDayHelpings(Foods.PumpkinSeeds_1_Cup)),
            ]);
        }
    }

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
                .Where(h => h.Helping.Food.Equals(food)).Sum(h => h.Helping.Servings * h.Multiplier));
}
