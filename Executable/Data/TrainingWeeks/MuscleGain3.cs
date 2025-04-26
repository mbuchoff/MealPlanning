using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain3 : TrainingWeekBase
{
    internal MuscleGain3() : base(
        "Muscle Gain 3",
        nonworkoutMeals:
        [
            new("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60),
                new("Blueberry oatmeal shake",
                    [new(Foods.BlueBerries_1_Scoop, 3)],
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
            new ("3-5 hours after last meal",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Ezekial()),
            new("Bedtime",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new("Shake",
                    [new(Foods.AlmondMilk_2_Cup, 1)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekial(withEdamame: false)),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 80),
                FoodGroupings.WorkoutShake),
            new("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 3, withEdamame: false)),
        ],
        xfitMeals:
        [
            new("Waking",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("Meal 2",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekial(withEdamame: false)),
            new("Meal 3",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 80),
                FoodGroupings.WorkoutShake),
            new("Bedtime",
                new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 3, withEdamame: false)),
        ])
    {

    }
}
