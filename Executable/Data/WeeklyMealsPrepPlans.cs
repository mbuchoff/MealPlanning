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
        }).SelectMany(x => x.MealsWithCounts.Select(mc =>
        {
            var allServings = mc.Meal.Servings.Where(s => !s.IsConversion);
            var cookingServings = allServings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.WithMeal);
            var eatingServings = allServings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime);

            return new MealPrepPlan(
                $"{x.TrainingDayType} - {mc.Meal.Name}",
                cookingServings,
                eatingServings,
                mc.MealCount,
                mc.Meal.Macros,
                mc.Meal.HasConversionFoods);
        })));
}
