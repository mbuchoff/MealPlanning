namespace SystemOfEquations;

public class MealPrepPlans
{
    public static MealPrepPlan Phase1MealPrepPlan
    {
        get
        {
            var runningDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Running day");
            var xfitDay = TrainingDays.Phase1TrainingDays.Single(td => td.Name == "Crossfit day");

            var runningDayHelpings = runningDay.Meals.SelectMany(m => m.Helpings);
            var xfitDayHelpings = xfitDay.Meals.SelectMany(m => m.Helpings);

            var runningBrownRice = runningDayHelpings.Single(h => h.Food.Name == Foods.BrownRice_1_Cup.Name);
            var xfitBrownRice = xfitDayHelpings.Single(h => h.Food.Name == Foods.BrownRice_1_Cup.Name);

            var runningSeitan = runningDayHelpings.Single(h => h.Food.Name == Foods.Seitan_Yeast_1_Tbsp_Gluten_2x.Name);
            var xfitSeitan = xfitDayHelpings.Single(h => h.Food.Name == Foods.Seitan_Yeast_1_Tbsp_Gluten_2x.Name);
            var seitanHelping = new Helping(
                Foods.Seitan_Yeast_1_Tbsp_Gluten_2x,
                runningSeitan.Servings * 2 + xfitSeitan.Servings * 3);

            var runningFarroServings = runningDayHelpings.Where(h => h.Food.Name == Foods.Farro_1_Cup.Name).Sum(h => h.Servings);
            var runningFarroHelping = new Helping(Foods.Farro_1_Cup, runningFarroServings);
            var xfitFarroServings = xfitDayHelpings.Where(h => h.Food.Name == Foods.Farro_1_Cup.Name).Sum(h => h.Servings);
            var xfitFarroHelping = new Helping(Foods.Farro_1_Cup, xfitFarroServings);

            var runningTofuServings = runningDayHelpings.Where(h => h.Food.Name == Foods.Tofu_1_5_block.Name).Sum(h => h.Servings);
            var xfitTofuServings = xfitDayHelpings.Where(h => h.Food.Name == Foods.Tofu_1_5_block.Name).Sum(h => h.Servings);
            var tofuHelping = new Helping(
                Foods.Tofu_1_5_block,
                runningTofuServings * 2 + xfitTofuServings * 3);

            return new(
            [
                runningBrownRice * 2,
                xfitBrownRice * 3,

                runningFarroHelping * 2,
                xfitFarroHelping * 3,

                seitanHelping,
                tofuHelping,
            ]);
        }
    }
        

}
