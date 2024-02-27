namespace SystemOfEquations;

internal static class TrainingDays
{
    public static IEnumerable<TrainingDay> Phase1TrainingDays { get; } = [
        new(
            "Non-weight training day",
            [
                new("Waking", new(P: 40, F: 10, C: 25), FoodGroupings.Oatmeal),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Seitan),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 40, F: 20, C: 25), FoodGroupings.Tofu),
                new("Bedtime", new(P: 40, F: 25, C: 0), new(
                    Foods.Cassein_1_Scoop,
                    Foods.AlmondButter_1_Tbsp,  // Will come up slightly negative, subtract from first meal
                    Foods.Oatmeal_1_2_Cup))
            ]),
        new(
            "Running day",
            [
                new("1-3 hours before workout",
                    new Macros(P: 30, F: 20, C: 40) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                    FoodGroupings.ProteinShake),
                new("40 minutes after workout", new(P: 30, F: 10, C: 80), FoodGroupings.Seitan),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 40), FoodGroupings.Tofu),
                new("Bedtime", new(P: 30, F: 25, C: 25), FoodGroupings.Oatmeal),
            ]),
        new(
            "Crossfit day",
            [
                new("1-3 hours before workout",
                    new Macros(P: 30, F: 20, C: 50) - Foods.AlmondMilk_2_Cup.NutritionalInformation.Macros,
                    FoodGroupings.ProteinShake),
                new("40 minutes after workout", new(P: 30, F: 10, C: 100), FoodGroupings.Seitan),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 65), FoodGroupings.Tofu),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 50), FoodGroupings.Tofu),
                new("Bedtime", new(P: 30, F: 25, C: 35), FoodGroupings.Oatmeal),
            ])
    ];
}
