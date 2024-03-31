namespace SystemOfEquations.Constants;

internal static class TrainingWeeks
{
    public static TrainingWeek MuscleGain1TrainingWeek { get; } = new(
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 25), FoodGroupings.Oatmeal),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: 40, F: 25, C: 0) - Foods.AlmondButter_1_Tbsp.NutritionalInformation.Macros,
                new(Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.Oatmeal_Walmart_1_2_Cup))  // Will come up slightly negative, subtract from first meal
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 40) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 80), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 40), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 25), FoodGroupings.Oatmeal),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 50) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.Oatmeal),
        ]);

    public static TrainingWeek MuscleGain2TrainingWeek { get; } = new(
        nonworkoutMeals:
        [
            new("Waking", new(P: 40, F: 10, C: 60), FoodGroupings.Oatmeal),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 40, F: 20, C: 60), FoodGroupings.Tofu),
            new("Bedtime",
                new Macros(P: 40, F: 25, C: 0) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                new(Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.Oatmeal_Walmart_1_Scoop))  // Will come up slightly negative, subtract from first meal
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 50) - Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros,
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.Oatmeal),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: 30, F: 20, C: 80) - (Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2),
                FoodGroupings.ProteinShake),
            new("40 minutes after workout", new(P: 30, F: 10, C: 120), FoodGroupings.Seitan),
            new("2-4 hours after last meal", new(P: 30, F: 20, C: 100), FoodGroupings.Seitan),
            new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
            new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.Oatmeal),
        ]);

    public static TrainingWeek MuscleGain3TrainingWeek { get; } = new(
        nonworkoutMeals:
        [
            new("Waking", new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60), FoodGroupings.Oatmeal),
            new("3-5 hours after last meal",
                new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60),
                FoodGroupings.FarroAndEdamame),
            new("Bedtime",
                new Macros(P: PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                new(Foods.PeaProtein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,
                    Foods.Oatmeal_Walmart_1_Scoop)),  // Will come up slightly negative, subtract from first meal
        ],
        runningMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80) - (Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2),
                FoodGroupings.ProteinShake),
            new("40 minutes after workout",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120),
                FoodGroupings.Seitan),
            new("2-4 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50),
                FoodGroupings.FarroAndEdamame),
            new("Bedtime", new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 35), FoodGroupings.Oatmeal),
        ],
        xfitMeals:
        [
            new("1-3 hours before workout",
                new Macros(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80) - (Foods.OrangeJuice_1_Cup.NutritionalInformation.Macros * 2),
                FoodGroupings.ProteinShake),
            new("40 minutes after workout",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), FoodGroupings.Seitan),
            new("2-4 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.Seitan),
            new("3-5 hours after last meal",
                new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100),
                FoodGroupings.FarroAndEdamame),
            new("Bedtime", new(P: PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 25, C: 65), FoodGroupings.Oatmeal),
        ]);

    private const double TARGET_WEIGHT = 165;
    private const int MEALS_PER_DAY = 5;

    // +1 to include workout shake
    private const double PROTEIN_PER_MEAL_ON_WORKOUT_DAY = TARGET_WEIGHT / (MEALS_PER_DAY + 1);

    private const double PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY = TARGET_WEIGHT / MEALS_PER_DAY;
}
