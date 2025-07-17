using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks;

internal record MuscleGain2 : TrainingWeekBase
{
    internal MuscleGain2() : base(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            new("Waking", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60),
                new("Blueberry oatmeal shake",
                    [new(Foods.BlueBerries_1_Scoop, Servings: 3), new(Foods.AlmondMilk_2_Cup)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.FatToCarbConversion,
                    Foods.Oats_1_Scoop,
                    PreparationMethodEnum.PrepareAsNeeded)),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Ezekial(withEdamame : true)),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new("Bedtime",
                    [new(Foods.AlmondMilk_2_Cup, Servings: 1)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    PreparationMethodEnum.PrepareAsNeeded))
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            new("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 100), FoodGroupings.Ezekial(withEdamame: false)),
            new("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 65), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Seitan),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 1)),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.BlueberriesOatmealAndEdamame),
            new("1/2 shake during workout, 1/2 right after", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 0, C:35), FoodGroupings.WorkoutShake),
            new("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Ezekial(withEdamame: true)),
            new("Bedtime", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 1)),
        ])
    {

    }
}
