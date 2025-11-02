using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks.Base;

internal record BaseWorkingOutInMorning : TrainingWeekBase
{
    internal BaseWorkingOutInMorning() : base(
        "Base, working out in morning",
        nonworkoutMeals:
        [
            new Meal("Waking",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 25),
                    new FoodGrouping("Blueberry oatmeal shake",
                        [Foods.BlueBerries_1_Scoop * 3, Foods.AlmondMilk_2_Cup],
                        Foods.PeaProtein_1_Scoop,
                        Foods.ChiaSeeds_2_5_Tbsp,
                        Foods.Oats_1_Scoop,
                        PreparationMethodEnum.PrepareAsNeeded)),
                new Meal("3-5 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
                FoodGroupings.Ezekiel),
            new("3-5 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
                FoodGroupings.TofuAndWheatBerries),
            new ("3-5 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
                FoodGroupings.TofuAndWheatBerries),
            new Meal("Bedtime",
                new Macros(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new FoodGrouping("Shake",
                    [Foods.AlmondMilk_2_Cup],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 30),
                blueBerriesOatmealAndEdamame),
            new("1/2 shake during working, 1/2 right after",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 35),
                FoodGroupings.WorkoutShake),
            new("40 minutes after workout",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 55),
                cerealAndEnglishMuffins),
            new("2-4 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 40),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 30),
                FoodGroupings.Seitan),
            new Meal("Bedtime",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 20),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0, withEdamame: true)),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 40),
                blueBerriesOatmealAndEdamame),
            new("1/2 shake during working, 1/2 right after",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 35),
                FoodGroupings.WorkoutShake),
            new("40 minutes after workout",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 80),
                cerealAndEnglishMuffins),
            new("2-4 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 40),
                FoodGroupings.Seitan),
            new Meal("Bedtime",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 25),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0, withEdamame: true)),
        ])
    {

    }

    private static readonly FoodGrouping blueBerriesOatmealAndEdamame = new(
        "blueberries, oatmeal, and edamame",
        [Foods.BlueBerries_1_Scoop],
        Foods.Edamame_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    private static readonly FoodGrouping cerealAndEnglishMuffins = new(
        "Cereal and english muffin",
        [
            Foods.Ezekiel_English_Muffin * 2,
            Foods.AlmondMilk_1_Scoop * 2,
        ],
        Foods.PumpkinSeeds_1_Scoop,
        Foods.Edamame_1_Scoop,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);
}
