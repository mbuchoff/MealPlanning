using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain2 : TrainingWeekBase
{
    internal MuscleGain2() : base(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 60), FoodGroupings.BlueBerryOatmeal),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: 40, F: 25, C: 0) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                new("Bedtime",
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded))
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 50) - Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 80) - Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 120), FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 100), FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ])
    {

    }
}
