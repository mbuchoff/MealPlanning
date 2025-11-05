using SystemOfEquations.Data.TrainingWeeks;

namespace SystemOfEquations.Data;

internal class WeeklyMealsPrepPlans
{
    public static WeeklyMealsPrepPlan Phase2MealPrepPlan => CreateMealPrepPlan(new MuscleGain2());

    public static WeeklyMealsPrepPlan Phase3MealPrepPlan => CreateMealPrepPlan(new MuscleGain3());

    public static WeeklyMealsPrepPlan CreateMealPrepPlan(TrainingWeek trainingWeek)
    {
        var prepareInAdvancePlans = new[] { TrainingDayTypes.XfitDay, TrainingDayTypes.RunningDay, TrainingDayTypes.NonweightTrainingDay }
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
            }));

        var prepareAsNeededPlans = new[] { TrainingDayTypes.XfitDay, TrainingDayTypes.RunningDay, TrainingDayTypes.NonweightTrainingDay }
            .Select(trainingDayType => new
            {
                DaysPerWeek = trainingDayType.DaysTraining.Count,
                trainingWeek.TrainingDays.Single(td => td.TrainingDayType == trainingDayType).Meals,
                TrainingDayType = trainingDayType,
            }).Select(x => new
            {
                x.DaysPerWeek,
                MealsWithCounts = x.Meals
                    .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
                    .SumWithSameFoodGrouping(x.DaysPerWeek),
                TrainingDayType = x.TrainingDayType,
            }).SelectMany(x => x.MealsWithCounts.Select(mc =>
            {
                var allServings = mc.Meal.Servings.Where(s => !s.IsConversion);

                return new MealPrepPlan(
                    $"{x.TrainingDayType} - {mc.Meal.Name}",
                    [], // PrepareAsNeeded meals have no cooking servings
                    allServings, // All servings are eating servings
                    mc.MealCount,
                    mc.Meal.Macros,
                    mc.Meal.HasConversionFoods);
            }));

        return new(prepareInAdvancePlans.Concat(prepareAsNeededPlans));
    }
}
