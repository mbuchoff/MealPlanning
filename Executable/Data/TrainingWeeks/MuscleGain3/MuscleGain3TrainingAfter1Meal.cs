using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks.MuscleGain3;

internal record MuscleGain3TrainingAfter1Meal : TrainingWeekBase
{
    internal MuscleGain3TrainingAfter1Meal() : base(
        "Muscle Gain 3",
        nonworkoutMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60),
                NonworkoutWakingOatmealFoodGroupings),
            new("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Ezekiel),
            new("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                RestDayCookingFoodGrouping),
            new Meal("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                RestDayCookingFoodGrouping),
            new Meal("Bedtime",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                NonworkoutBedtimeFoodGroupings),
        ],
        runningMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.BlueberriesOatmealAndEdamame + Foods.Creatine_1_Scoop),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 55),
                FoodGroupings.WorkoutMeal),
            new Meal("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120),
                FoodGroupings.Seitan),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.Ezekiel),
            new Meal("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ],
        xfitMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                Oatmeal),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 55),
                FoodGroupings.WorkoutMeal),
            new Meal("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120),
                FoodGroupings.Ezekiel),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                EnglishMuffinsAndRice),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                EnglishMuffinsAndRice),
            new Meal("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 65),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ])
    {

    }

    private static readonly FallbackChain Oatmeal = new(
        new[] { Foods.Edamame_1_Scoop, Foods.ProteinToFatConversion }
            .Select(pFood => new FoodGrouping(
                "blueberries and oatmeal",
                [
                    Foods.Ezekiel_English_Muffin, Foods.AlmondButter_1_Tbsp, Foods.BlueBerries_1_Scoop * 4,
                    Foods.Creatine_1_Scoop
                ],
                pFood,
                Foods.ChiaSeeds_2_5_Tbsp,
                Foods.Oats_1_Scoop,
                PreparationMethodEnum.PrepareAsNeeded)).ToArray());

    private static FallbackChain EnglishMuffinsAndRice { get; } = new(
        new[] { Foods.WheatBerries_45_Grams, Foods.ProteinToCarbConversion }.Select(pFood => new FoodGrouping(
            "rice",
            [Foods.Ezekiel_English_Muffin * 2],
            pFood,
            Foods.PumpkinSeeds_30_Grams,
            Foods.BrownRice_45_Grams,
            PreparationMethodEnum.PrepareInAdvance)).ToArray());

    private static FallbackChain RestDayCookingFoodGrouping { get; } = new(
        new[]
        {
            new FoodGrouping("rice",
                [],
                Foods.WheatBerries_45_Grams,
                Foods.PumpkinSeeds_30_Grams,
                Foods.BrownRice_45_Grams,
                PreparationMethodEnum.PrepareInAdvance),
            FoodGroupings.Tofu
        });

    private static readonly FallbackChain NonworkoutWakingOatmealFoodGroupings = new(
        [.. new[] { Foods.AlmondButter_1_Tbsp, Foods.FatToCarbConversion }.Select(fFood =>
        new FoodGrouping("Blueberry oatmeal shake",
            [
                Foods.BlueBerries_1_Scoop * 3,
                Foods.Creatine_1_Scoop,
            ],
            Foods.Edamame_1_Scoop,
            fFood,
            Foods.Oats_1_Scoop,
            PreparationMethodEnum.PrepareAsNeeded))]);

    private static readonly FallbackChain NonworkoutBedtimeFoodGroupings = new(
        [.. new[] { Foods.Whole_Grain_Pasta_56_Grams, Foods.FatToCarbConversion }.Select(cFood =>
        new FoodGrouping("Protein shake",
            Foods.Edamame_1_Scoop,
            Foods.Almonds_1_Scoop,
            cFood,
            PreparationMethodEnum.PrepareAsNeeded))]);

}
