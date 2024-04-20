using SystemOfEquations.Constants;

namespace SystemOfEquations;

internal class MealPrepPlans
{
    public static MealPrepPlan Phase2MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain2TrainingWeek);

    public static MealPrepPlan Phase3MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain3TrainingWeek);

    public static MealPrepPlan CreateMealPrepPlan(TrainingWeek trainingWeek)
    {
        SumOfIndividualMeals(trainingWeek.XFitDay);

        return new(
        new[]
        {
            new { Multiplier = 3, TrainingType = TrainingDayTypes.XfitDay },
            new { Multiplier = 2, TrainingType = TrainingDayTypes.RunningDay },
            new { Multiplier = 1, TrainingType = TrainingDayTypes.NonweightTrainingDay },
        }.Select(x => new
        {
            Day = trainingWeek.TrainingDays.Single(td =>
                td.TrainingDayType == x.TrainingType),
            x.Multiplier,
            x.TrainingType,
        }).SelectMany(x => SumOfIndividualMeals(x.Day)
            .Select(m => new
            {
                x.Multiplier,
                Meal = m,
                x.TrainingType,
            }))
        .OrderBy(x => x.Meal.FoodGrouping.Name)
        .SelectMany(x => x.Meal.Helpings
            .Where(h => !_foodsExcludedFromMealPrepPlan.Contains(h.Food))
            .Select(h => ($"{x.TrainingType} - {x.Meal.FoodGrouping}", h * x.Multiplier))));
    }

    private static IEnumerable<Meal> SumOfIndividualMeals(TrainingDay trainingDay)
    {
        var mealGroups = trainingDay.Meals.GroupBy(m => m.FoodGrouping);
        var groupedHelpings = mealGroups.Select(mealGroup => CombineLikeHelpings(mealGroup.SelectMany(m => m.Helpings)));
        var meals = mealGroups.Select(mealGroup => new Meal(mealGroup.Key.Name,
            mealGroup.Sum(m => m.Macros),
            mealGroup.Key));
        return meals;
    }

    private static IEnumerable<Helping> CombineLikeHelpings(IEnumerable<Helping> helpings)
    {
        var foodGroupings = helpings.GroupBy(h => h.Food);
        var combinedHelpings =
            foodGroupings.Select(foodGrouping => new Helping(
                Food: foodGrouping.Key,
                Servings: foodGrouping.Select(h => h.Servings).Sum()));
        return combinedHelpings;
    }

    private readonly static IEnumerable<Food> _foodsExcludedFromMealPrepPlan = [
        Foods.AlmondButter_1_Tbsp,
        Foods.Blueberries_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Edamame_1_Scoop,
        Foods.Oatmeal_Sprouts_1_Scoop,
        Foods.PeaProtein_1_Scoop,
    ];
}
