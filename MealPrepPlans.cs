namespace SystemOfEquations;

public class MealPrepPlans
{
    public static MealPrepPlan Phase1MealPrepPlan
    {
        get
        {
            var runningDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Running day");
            var xfitDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Crossfit day");

            Helping ConsolidateHelpings(Food food) =>
                MealPrepPlans.ConsolidateHelpings([(runningDay, 2), (xfitDay, 3)], food.Name);

            var runningDayHelpings = runningDay.Meals.SelectMany(m => m.Helpings);
            var xfitDayHelpings = xfitDay.Meals.SelectMany(m => m.Helpings);

            var runningBrownRiceHelping = runningDayHelpings.Single(h => h.Food.Name == Foods.BrownRice_1_Cup.Name);
            var xfitBrownRiceHelping = xfitDayHelpings.Single(h => h.Food.Name == Foods.BrownRice_1_Cup.Name);

            var runningFarroServings = runningDayHelpings.Where(h => h.Food.Name == Foods.Farro_1_Cup.Name).Sum(h => h.Servings);
            var runningFarroHelping = new Helping(Foods.Farro_1_Cup, runningFarroServings);
            
            var xfitFarroServings = xfitDayHelpings.Where(h => h.Food.Name == Foods.Farro_1_Cup.Name).Sum(h => h.Servings);
            var xfitFarroHelping = new Helping(Foods.Farro_1_Cup, xfitFarroServings);

            return new(
            [
                runningBrownRiceHelping * 2,
                xfitBrownRiceHelping * 3,

                runningFarroHelping * 2,
                xfitFarroHelping * 3,

                ConsolidateHelpings(Foods.Seitan_Yeast_1_Tbsp_Gluten_2x),
                ConsolidateHelpings(Foods.OliveOil_1_Tbsp),
                ConsolidateHelpings(Foods.Tofu_1_5_block),
                ConsolidateHelpings(Foods.PumpkinSeeds_1_Cup),
            ]);
        }
    }

    private static Helping ConsolidateHelpings(IEnumerable<(TrainingDay TrainingDay, double Multiplier)> trainingDays, string food)
    {
        var helpings = trainingDays
            .SelectMany(t => t.TrainingDay.Meals.Select(m => (Meal: m, t.Multiplier)))
            .SelectMany(m => m.Meal.Helpings.Select(h => (Helping: h, m.Multiplier)))
            .Where(h => h.Helping.Food.Name == food).ToList();
        return new(
            helpings.First().Helping.Food,
            helpings.Sum(h => h.Helping.Servings * h.Multiplier));
    }
}
