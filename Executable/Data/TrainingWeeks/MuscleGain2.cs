using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain2 : TrainingWeekBase
{
    internal MuscleGain2() : base(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 60),
                new("Blueberry oatmeal shake",
                    [new(Foods.BlueBerries_1_Scoop, Servings: 3), new(Foods.AlmondMilk_2_Cup)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.FatToCarbConversion,
                    Foods.Oats_1_Scoop,
                    PreparationMethodEnum.PrepareAsNeeded)),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Ezekial(withEdamame : true)),
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
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Ezekial(withEdamame: false)),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Seitan),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 1)),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 80) - Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2,
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("40 minutes after workout", new(P: 30, F: 10, C: 120), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 100), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Ezekial(withEdamame: true)),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 1)),
        ])
    {

    }

    private static readonly FoodGrouping cerealAndEnglishMuffins = new(
        "Cereal and english muffin",
        [
            new(Foods.Ezekiel_English_Muffin, Servings: 1),
                new(Foods.AlmondMilk_1_Scoop, Servings: 1),
        ],
        Foods.PumpkinSeeds_1_Scoop,
        Foods.Edamame_1_Scoop,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);
}
