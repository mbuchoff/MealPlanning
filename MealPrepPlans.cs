using SystemOfEquations.Data;

namespace SystemOfEquations;

internal class MealPrepPlans
{
    public static MealPrepPlan Phase2MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain2TrainingWeek);

    public static MealPrepPlan Phase3MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain3TrainingWeek);

    public static MealPrepPlan CreateMealPrepPlan(TrainingWeek trainingWeek) => new(
        new[] { TrainingDayTypes.XfitDay, TrainingDayTypes.RunningDay, TrainingDayTypes.NonweightTrainingDay }
        .Select(trainingType => new
        {
            trainingType.MealPrepsPerWeek,
            trainingWeek.TrainingDays.Single(td =>
                td.TrainingDayType == trainingType).Meals,
            TrainingType = trainingType,
        }).SelectMany(x => x.Meals
            .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
            .SumWithSameFoodGrouping()
            .Select(m => new
            {
                x.MealPrepsPerWeek,
                Meal = m,
                x.TrainingType,
            }))
        .OrderBy(x => x.Meal.FoodGrouping.Name)
        .SelectMany(x => x.Meal.Helpings
            .Where(h => !_foodsExcludedFromMealPrepPlan.Contains(h.Food))
            .Select(h => (Description: $"{x.TrainingType}: {x.Meal.FoodGrouping}", Helping: h * x.MealPrepsPerWeek))));

    private readonly static IEnumerable<Food> _foodsExcludedFromMealPrepPlan = [
        Foods.AlmondButter_1_Tbsp,
        Foods.BlueBerries_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oatmeal_Sprouts_1_Scoop,
        Foods.PeaProtein_1_Scoop,
    ];
}
