using SystemOfEquations.Data;

namespace SystemOfEquations;

internal class MealPrepPlans
{
    public static MealPrepPlan Phase2MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain2TrainingWeek);

    public static MealPrepPlan Phase3MealPrepPlan => CreateMealPrepPlan(TrainingWeeks.MuscleGain3TrainingWeek);

    public static MealPrepPlan CreateMealPrepPlan(TrainingWeek trainingWeek) => new(
        new[] { TrainingDayTypes.XfitDay, TrainingDayTypes.RunningDay, TrainingDayTypes.NonweightTrainingDay }
        .Select(trainingDayType => new
        {
            trainingDayType.DaysMealPrepping,
            trainingWeek.TrainingDays.Single(td => td.TrainingDayType == trainingDayType).Meals,
            TrainingDayType = trainingDayType,
        }).Select(x => new
        {
            x.DaysMealPrepping,
            Meals = x.Meals
                .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
                .SumWithSameFoodGrouping()
                .Select(m => new Meal($"{x.TrainingDayType} - {m.Name}", m.Macros, m.FoodGrouping)),
            x.TrainingDayType,
        }).SelectMany(x => x.Meals.Select(m => (
            m.Name,
            Helpings: m.Helpings
                .Where(h => !_foodsExcludedFromMealPrepPlan.Contains(h.Food))
                .Select(h => h * x.DaysMealPrepping)))));

    private readonly static IEnumerable<Food> _foodsExcludedFromMealPrepPlan = [
        Foods.AlmondButter_1_Tbsp,
        Foods.BlueBerries_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oatmeal_Sprouts_1_Scoop,
        Foods.PeaProtein_1_Scoop,
    ];
}
