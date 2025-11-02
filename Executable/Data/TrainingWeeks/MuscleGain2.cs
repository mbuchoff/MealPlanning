using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain2 : TrainingWeekBase
{
    internal MuscleGain2() : base(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            new Meal("Waking", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60), WakingBlueberryOatmealShakeFoodGroupings),
            new Meal("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Ezekiel),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            new Meal("Bedtime", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0), BedtimeProteinShakeFoodGroupings)
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.BlueberriesOatmealAndEdamame + Foods.Creatine_1_Scoop),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            new Meal("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 100), FoodGroupings.Ezekiel),
            new("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 65), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Seitan),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ],
        xfitMeals:
        [
            new Meal("1-3 hours before workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80), Oatmeal),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            new Meal("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), WheatBerriesAndRice),
            new Meal("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100), WheatBerriesAndRice),
            new Meal("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Ezekiel),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ])
    {

    }

    private static readonly FallbackChain Oatmeal = new(
        new[] { Foods.Edamame_1_Scoop, Foods.ProteinToFatConversion }
            .Select(pFood => new FoodGrouping(
            "blueberries and oatmeal",
            [Foods.Ezekiel_English_Muffin, Foods.BlueBerries_1_Scoop * 4, Foods.Creatine_1_Scoop],
            pFood,
            Foods.ChiaSeeds_2_5_Tbsp,
            Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded)).ToArray());

    private static FallbackChain WheatBerriesAndRice { get; } = new(
        new[] { Foods.WheatBerries_45_Grams, Foods.ProteinToCarbConversion }.Select(pFood => new FoodGrouping(
            "rice",
            [Foods.Ezekiel_English_Muffin * 1],
            pFood,
            Foods.PumpkinSeeds_30_Grams,
            Foods.BrownRice_45_Grams,
            PreparationMethodEnum.PrepareInAdvance)).ToArray());

    private static readonly FallbackChain WakingBlueberryOatmealShakeFoodGroupings = new(
        [.. new[] { Foods.AlmondButter_1_Tbsp, Foods.FatToCarbConversion }.Select(fFood =>
        new FoodGrouping("Blueberry oatmeal shake",
            [
                Foods.BlueBerries_1_Scoop * 3,
                Foods.AlmondMilk_2_Cup,
                Foods.Creatine_1_Scoop,
            ],
            Foods.PeaProtein_1_Scoop,
            fFood,
            Foods.Oats_1_Scoop,
            PreparationMethodEnum.PrepareAsNeeded))]);

    private static readonly FallbackChain BedtimeProteinShakeFoodGroupings = new(
        [.. new[] { Foods.BlueBerries_1_Scoop, Foods.FatToCarbConversion }.Select(cFood =>
        new FoodGrouping("Protein shake",
            [Foods.AlmondMilk_2_Cup],
            Foods.PeaProtein_1_Scoop,
            Foods.AlmondButter_1_Tbsp,
            cFood,
            PreparationMethodEnum.PrepareAsNeeded))]);
}
