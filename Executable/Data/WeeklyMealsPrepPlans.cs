using SystemOfEquations.Data.TrainingWeeks;

namespace SystemOfEquations.Data;

internal class WeeklyMealsPrepPlans
{
    public static WeeklyMealsPrepPlan Phase2MealPrepPlan => CreateMealPrepPlan(new MuscleGain2());

    public static WeeklyMealsPrepPlan Phase3MealPrepPlan => CreateMealPrepPlan(new MuscleGain3());

    public static WeeklyMealsPrepPlan CreateMealPrepPlan(TrainingWeek trainingWeek) => new(
        new[] { TrainingDayTypes.XfitDay, TrainingDayTypes.RunningDay, TrainingDayTypes.NonweightTrainingDay }
        .Select(trainingDayType => new
        {
            trainingDayType.DaysEatingPreparedMeals,
            trainingWeek.TrainingDays.Single(td => td.TrainingDayType == trainingDayType).Meals,
            TrainingDayType = trainingDayType,
        }).Select(x => new
        {
            x.DaysEatingPreparedMeals,
            MealsWithCounts = x.Meals
                .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
                .SumWithSameFoodGrouping(x.DaysEatingPreparedMeals),
            TrainingDayType = x.TrainingDayType,
        }).SelectMany(x => x.MealsWithCounts.Select(mc => new MealPrepPlan(
            $"{x.TrainingDayType} - {mc.Meal.Name}",
            mc.Meal.Servings
                .Where(s => !_foodsExcludedFromMealPrepPlan.Any(excluded => excluded.Name == s.Name)),
            mc.MealCount))));

    private readonly static IEnumerable<FoodServing> _foodsExcludedFromMealPrepPlan = [
        Foods.AlmondButter_1_Tbsp,
        Foods.BlueBerries_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oats_1_Scoop,
        Foods.PeaProtein_1_Scoop,
    ];
}
