using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record FatLoss1 : TrainingWeekBase
{
    internal FatLoss1() : base(
        "Base",
        nonworkoutMeals:
        [
            new("Waking",
                new(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 0, C: 25),
                BlueBerryShakeTwice),
            new("3-5 hours after last meal",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 0, C: 25),
                BlueBerryShakeTwice),
            new("3-5 hours after last meal",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 0, C: 25),
                JustTofu),
            new("3-5 hours after last meal",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 25),
                JustTofu),
            new("Bedtime",
                new Macros(P: FAT_LOSS_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new("Shake",
                    [Foods.AlmondMilk_2_Cup],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new ("Waking",
            new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 20),
                BlueBerryShakeTwice),
            new ("Meal 2",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 20),
                    BlueBerryShakeTwice),
            new ("Meal 3",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 20),
                new ("Edamame",
                    Foods.Edamame_1_Scoop,
                    Foods.FatToCarbConversion,
                    Foods.ProteinToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded)),
            new ("Meal 4",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 15, C: 40),
                Seitan),
            new ("1/2 shake during working, 1/2 right after",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 65),
                FoodGroupings.WorkoutShake),
            new ("Bedtime",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 15, C: 55),
                FoodGroupings.Seitan),
        ],
        xfitMeals:
        [
            new ("Waking",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 25),
                BlueBerryShakeTwice),
            new ("Meal 2",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 25),
                BlueBerryShakeTwice),
            new ("Meal 3",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 25),
                JustEdamame),
            new ("Meal 4",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 15, C: 50),
                Tofu),
            new ("1/2 shake during working, 1/2 right after",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 65),
                FoodGroupings.WorkoutShake),
            new ("Bedtime",
                new (P: FAT_LOSS_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 15, C: 80),
                Tofu),
        ])
    {
    }

    private static readonly FoodGrouping JustTofu = new(
        "Tofu",
        Foods.Tofu_91_Grams,
        Foods.FatToCarbConversion,
        Foods.ProteinToCarbConversion,
        PreparationMethodEnum.PrepareInAdvance);

    private static readonly FoodGrouping BlueBerryShakeTwice = new(
        "Blueberry shake (make twice as much)",
        Foods.PeaProtein_1_Scoop,
        Foods.FatToCarbConversion,
        Foods.BlueBerries_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    private static readonly FoodGrouping Seitan = new(
        "seitan",
        Foods.Seitan_Sprouts_Yeast_1_Gram_Gluten_4x,
        Foods.FatToCarbConversion,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    private static readonly FoodGrouping JustEdamame = new(
        "Edamame",
        Foods.Edamame_1_Scoop,
        Foods.FatToCarbConversion,
        Foods.ProteinToCarbConversion,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Tofu { get; } = new(
        "tofu",
        Foods.Tofu_91_Grams,
        Foods.FatToProteinConversion,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);
}
