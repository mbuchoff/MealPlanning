using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain1 : TrainingWeekBase
{
    internal MuscleGain1() : base(
        "Muscle Gain 1",
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 25), FoodGroupings.BlueBerryOatmeal),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Seitan),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
                new("Bedtime",
                    new Macros(P: 40, F: 25, C: 0) - Foods.AlmondButter_1_Tbsp.NutritionalInformation.Macros,
                    new("Bedtime",
                        Foods.PeaProtein_1_Scoop,
                        Foods.AlmondButter_1_Tbsp,
                        Foods.FatToCarbConversion,
                        PreparationMethodEnum.PrepareAsNeeded))
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                    new Macros(P: 30, F: 20, C: 40) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                    FoodGroupings.ProteinShake),
                new("40 minutes after workout", new(P: 30, F: 10, C: 80), FoodGroupings.Seitan),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 40), FoodGroupings.Tofu),
                new("Bedtime", new(P: 30, F: 25, C: 25), FoodGroupings.OatmealWithAlmondButter),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                    new Macros(P: 30, F: 20, C: 50) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                    FoodGroupings.ProteinShake),
                new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
                new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ])
    {

    }
}
