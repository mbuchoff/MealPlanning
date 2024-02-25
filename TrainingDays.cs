namespace SystemOfEquations;

public static class TrainingDays
{
    public static IEnumerable<TrainingDay> Phase1TrainingDays { get; } = [
        new(
            "Non-weight training day",
            [
                new("Waking", new(P: 40, F: 10, C: 25),
                    pFood: Foods.Edamame_1_4_Cup,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.Oatmeal_1_2_Cup),
                new("Bedtime", new(P: 40, F: 25, C: 0),
                    pFood: Foods.Cassein_1_Scoop,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.Oatmeal_1_2_Cup), // Will come up slightly negative, subtract from first meal
            ]),
        new(
            "Running day",
            [
                new("1-3 hours before workout", new Macros(P: 30, F: 20, C: 40) - Foods.AlmondMilk_2_Cup.Macros,
                    pFood: Foods.Whey_1_Scoop,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.BlueBerries_1_Cup),
                new("40 minutes after workout", new(P: 30, F: 10, C: 80),
                    pFood: Foods.Seitan_Yeast_1_Tbsp_Gluten_2x,
                    fFood: Foods.OliveOil_1_Tbsp,
                    cFood: Foods.BrownRice_1_Cup),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 50),
                    pFood: Foods.Tofu_1_5_block,
                    fFood: Foods.PumpkinSeeds_1_Cup,
                    cFood: Foods.Farro_1_Cup),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 40),
                    pFood: Foods.Tofu_1_5_block,
                    fFood: Foods.PumpkinSeeds_1_Cup,
                    cFood: Foods.Farro_1_Cup),
                new("Bedtime", new(P: 30, F: 25, C: 25),
                    pFood: Foods.Edamame_1_4_Cup,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.Oatmeal_1_2_Cup),
            ]),
        new(
            "Crossfit day",
            [
                new("1-3 hours before workout", new Macros(P: 30, F: 20, C: 50) - Foods.AlmondMilk_2_Cup.Macros,
                    pFood: Foods.Whey_1_Scoop,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.BlueBerries_1_Cup),
                new("40 minutes after workout", new(P: 30, F: 10, C: 100),
                    pFood: Foods.Seitan_Yeast_1_Tbsp_Gluten_2x,
                    fFood: Foods.OliveOil_1_Tbsp,
                    cFood: Foods.BrownRice_1_Cup),
                new("2-4 hours after last meal", new(P: 30, F: 20, C: 65),
                    pFood: Foods.Tofu_1_5_block,
                    fFood: Foods.PumpkinSeeds_1_Cup,
                    cFood: Foods.Farro_1_Cup),
                new("3-5 hours after last meal", new(P: 30, F: 20, C: 50),
                    pFood: Foods.Tofu_1_5_block,
                    fFood: Foods.PumpkinSeeds_1_Cup,
                    cFood: Foods.Farro_1_Cup),
                new("Bedtime", new(P: 30, F: 25, C: 35),
                    pFood: Foods.Edamame_1_4_Cup,
                    fFood: Foods.AlmondButter_1_Tbsp,
                    cFood: Foods.Oatmeal_1_2_Cup),
            ])
    ];
}
