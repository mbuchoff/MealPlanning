﻿using SystemOfEquations.Data.TrainingWeeks;

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
            Meals = x.Meals
                .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
                .SumWithSameFoodGrouping(x.DaysEatingPreparedMeals)
                .Select(m => new Meal($"{x.TrainingDayType} - {m.Name}", m.Macros, m.FoodGrouping)),
        }).SelectMany(x => x.Meals.Select(m => new MealPrepPlan(m.Name,
            m.Helpings
                .Where(h => !_foodsExcludedFromMealPrepPlan.Contains(h.Food))
                .Select(h => h * x.DaysEatingPreparedMeals)))));

    private readonly static IEnumerable<Food> _foodsExcludedFromMealPrepPlan = [
        Foods.AlmondButter_1_Tbsp,
        Foods.BlueBerries_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oats_1_Scoop,
        Foods.PeaProtein_1_Scoop,
    ];
}
