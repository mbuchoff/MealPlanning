﻿using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks.Base;

internal record BaseWorkingOutInEvening : TrainingWeekBase
{
    internal BaseWorkingOutInEvening() : base(
        "Base",
        nonworkoutMeals:
        [
            new("Waking",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 25),
                new("Blueberry oatmeal shake",
                    [new(Foods.BlueBerries_1_Scoop, Servings: 3), new(Foods.AlmondMilk_2_Cup)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.ChiaSeeds_2_5_Tbsp,
                    Foods.Oats_1_Scoop,
                    PreparationMethodEnum.PrepareAsNeeded)),
            new("3-5 hours after last meal",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
            FoodGroupings.Ezekial()),
        new("3-5 hours after last meal",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
            FoodGroupings.TofuAndWheatBerries),
        new ("3-5 hours after last meal",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 25),
            FoodGroupings.TofuAndWheatBerries),
        new("Bedtime",
            new Macros(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
            new("Shake",
                [new(Foods.AlmondMilk_2_Cup)],
                Foods.PeaProtein_1_Scoop,
                Foods.AlmondButter_1_Tbsp,
                Foods.FatToCarbConversion,
                PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new("Waking",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 20),
             new("blueberries, oatmeal, and edamame",
                [new(Foods.BlueBerries_1_Scoop)],
                Foods.Edamame_1_Scoop,
                Foods.AlmondButter_1_Tbsp,
                Foods.Oats_1_Scoop,
                PreparationMethodEnum.PrepareAsNeeded)),
        new("Meal 2",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 20),
            FoodGroupings.Ezekial(withEdamame: true)),
        new("Meal 3",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 20),
            FoodGroupings.Seitan),
        new("Meal 4",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 40),
            FoodGroupings.Seitan),
        new("1/2 shake during working, 1/2 right after",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 65),
            FoodGroupings.WorkoutShake),
        new("Bedtime",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 55),
            FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 2, withEdamame: true)),
        ],
        xfitMeals:
        [
            new("Waking",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 25),
             new("blueberries, oatmeal, and edamame",
                [new(Foods.BlueBerries_1_Scoop, Servings: 2)],
                Foods.Edamame_1_Scoop,
                Foods.AlmondButter_1_Tbsp,
                Foods.Oats_1_Scoop,
                PreparationMethodEnum.PrepareAsNeeded)),
        new("Meal 2",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 25),
            FoodGroupings.Ezekial(withEdamame: true)),
        new("Meal 3",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 25),
            FoodGroupings.Tofu),
        new("Meal 4",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
            FoodGroupings.Tofu),
        new("1/2 shake during working, 1/2 right after",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 65),
            FoodGroupings.WorkoutShake),
        new("Bedtime",
            new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
            FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 3, withEdamame: false)),
        ])
    {

    }
}
