namespace SystemOfEquations.Data;

internal static class TrainingWeeks
{
    public static TrainingWeek MuscleGain1TrainingWeek => new(
        "Muscle Gain 1",
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 25), FoodGroupings.BlueBerryOatmeal),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: 40, F: 25, C: 0) - Foods.AlmondButter_1_Tbsp.NutritionalInformation.Macros,
                new("Bedtime",
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 40) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 80), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 40), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 25), FoodGroupings.OatmealWithAlmondButter),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 50) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ]);

    public static TrainingWeek MuscleGain2TrainingWeek => new(
        "Muscle Gain 2",
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 60), FoodGroupings.BlueBerryOatmeal),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: 40, F: 25, C: 0) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                new("Bedtime",
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 50) - Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 80) - (Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2),
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 120), FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 100), FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.OatmealWithAlmondButter),
        ]);

    public static TrainingWeek MuscleGain3TrainingWeek => new(
        "Muscle Gain 3",
        nonworkoutMeals:
        [
            new("Waking",
                new(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60),
                new("Blueberry oatmeal shake",
                    [new(Foods.BlueBerries_1_Scoop, 3)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondMilk_2_Cup,
                    Foods.Oats_1_Scoop,
                    FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)),
            new("3-5 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new("3-5 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.PearledBarley_BlackBeans_PumpkinSeeds),
            new ("3-5 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Ezekial()),
            new("Bedtime",
                new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0),
                new("Shake",
                    [new(Foods.AlmondMilk_2_Cup, 1)],
                    Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.FatToCarbConversion,
                    FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)),
        ],
        runningMeals:
        [
            new("Waking",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.ApplesBlueberriesOatmealAndEdamame),
            new("Meal 2",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekial(withEdamame: false)),
            new("Meal 3",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 55),
                FoodGroupings.WorkoutShake),
            new("Bedtime",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.Oatmeal(withEdamame: false)),
        ],
        xfitMeals:
        [
            new("Waking",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.ApplesBlueberriesOatmealAndEdamame),
            new("Meal 2",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Ezekial(withEdamame: false)),
            new("Meal 3",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80),
                FoodGroupings.Seitan),
            new("Meal 4",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("1/2 shake during working, 1/2 right after",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 0, C: 55),
                FoodGroupings.WorkoutShake),
            new("Bedtime",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 120),
                FoodGroupings.Oatmeal(withEdamame: false)),
        ]);

    internal static TrainingWeek MuscleGain3PlusPercentTrainingWeek(double percent) =>
        MuscleGain3TrainingWeek.CloneWithTweakedMacros(
            pMultiplier: 1,
            fMultiplier: percent / 100,
            cMultiplier: percent / 100);

    private const double TARGET_WEIGHT = 170;
    private const int MEALS_PER_DAY = 5;

    // +1 to include workout shake
    private const double PROTEIN_PER_MEAL_ON_WORKOUT_DAY = TARGET_WEIGHT / (MEALS_PER_DAY + 1);

    private const double PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY = TARGET_WEIGHT / MEALS_PER_DAY;
}
