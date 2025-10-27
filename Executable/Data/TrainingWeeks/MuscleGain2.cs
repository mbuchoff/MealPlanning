using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain2 : TrainingWeekBase
{
    internal MuscleGain2() : base(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            Meal.WithFallbacks("Waking", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60), WakingBlueberryOatmealShakeFoodGroupings),
            Meal.WithFallbacks("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Ezekial),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            Meal.WithFallbacks("Bedtime", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0), BedtimeProteinShakeFoodGroupings)
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.BlueberriesOatmealAndEdamame + Foods.Creatine_1_Scoop),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            Meal.WithFallbacks("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 100), FoodGroupings.Ezekial),
            new("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 65), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Seitan),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ],
        xfitMeals:
        [
            Meal.WithFallbacks("1-3 hours before workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80), Oatmeal),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            Meal.WithFallbacks("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), WheatBerriesAndRice),
            Meal.WithFallbacks("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100), WheatBerriesAndRice),
            Meal.WithFallbacks("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Ezekial),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
        ])
    {

    }

    private static readonly FoodGrouping[] Oatmeal =
        new[] { Foods.Edamame_1_Scoop, Foods.ProteinToFatConversion }
            .Select(pFood => new FoodGrouping(
            "blueberries and oatmeal",
            [Foods.Ezekiel_English_Muffin, Foods.BlueBerries_1_Scoop * 4, Foods.Creatine_1_Scoop],
            pFood,
            Foods.ChiaSeeds_2_5_Tbsp,
            Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded)).ToArray();

    private static FoodGrouping[] WheatBerriesAndRice { get; } =
        new[] { Foods.BrownRice_45_Grams, Foods.ProteinToCarbConversion }.Select(cFood => new FoodGrouping(
            "wheat berries",
            [Foods.Ezekiel_English_Muffin * 1],
            Foods.WheatBerries_45_Grams,
            Foods.PumpkinSeeds_30_Grams,
            cFood,
            PreparationMethodEnum.PrepareInAdvance)).ToArray();

    private static readonly FoodGrouping[] WakingBlueberryOatmealShakeFoodGroupings =
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
            PreparationMethodEnum.PrepareAsNeeded))];

    private static readonly FoodGrouping[] BedtimeProteinShakeFoodGroupings =
        [.. new[] { Foods.BlueBerries_1_Scoop, Foods.FatToCarbConversion }.Select(cFood =>
        new FoodGrouping("Protein shake",
            [Foods.AlmondMilk_2_Cup],
            Foods.PeaProtein_1_Scoop,
            Foods.AlmondButter_1_Tbsp,
            cFood,
            PreparationMethodEnum.PrepareAsNeeded))];
}
