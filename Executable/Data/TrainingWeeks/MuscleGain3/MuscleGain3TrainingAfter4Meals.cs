using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain3TrainingAfter4Meals : TrainingWeekBase
{
    internal MuscleGain3TrainingAfter4Meals() : base(
        "Muscle Gain 3",
        nonworkoutMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60),
                new FoodGrouping("Blueberry oatmeal shake",
                    [Foods.BlueBerries_1_Scoop * 3],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondMilk_2_Cup,
                    Foods.Oats_1_Scoop,
                    PreparationMethodEnum.PrepareAsNeeded)),
            new("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new Meal("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Ezekiel),
            new Meal("Bedtime",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new FoodGrouping("Shake",
                    [Foods.AlmondMilk_2_Cup],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new Meal("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekiel),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 80),
                FoodGroupings.WorkoutShake),
            new Meal("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 3, withEdamame: false)),
        ],
        xfitMeals:
        [
            new Meal("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new Meal("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekiel),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 80),
                FoodGroupings.WorkoutShake),
            new Meal("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 3, withEdamame: false)),
        ])
    {

    }
}
