using System.Data;

namespace SystemOfEquations.Constants;

internal static class Foods
{
    public static Food AlmondButter_1_Tbsp { get; } = new(
        "almond butter",
        new(1, ServingUnits.Tablespoon, Cals: 130, new(P: 4.5, F: 11.5, C: 4.5 - 2.5)));
    public static Food AlmondMilk_2_Cup { get; } = new(
        "almond milk",
        new(2, ServingUnits.Cup, Cals: 59, new(P: 2, F: 5.1, C: 2 - 2)));
    public static Food BlueBerries_1_Cup { get; } = new(
        "frozen blueberries",
        new(1, ServingUnits.Cup, Cals: 80, new(P: 0, F: 0, C: 19 - 6)));
    public static Food BrownRice_45_Grams { get; } = new(
        "brown rice",
        new(
            // 0.25, ServingUnits.Cup
            45, ServingUnits.Gram,
            Cals: 170,
            new(P: 4, F: 1.5, C: 35 - 2)));
    public static Food Cassein_1_Scoop { get; } = new(
        "cassein",
        new(1, ServingUnits.Scoop, Cals: 100, new(P: 25, F: 0, C: 1 - 0)));
    public static Food ChiaSeeds_2_5_Tbsp { get; } = new(
        "chia seeds",
        new(2.5, ServingUnits.Tablespoon, Cals: 150, new(P: 5, F: 9, C: 13 - 10)));
    public static Food Edamame_1_4_Cup { get; } = new(
        "edamame",
        new(0.25, ServingUnits.Cup, Cals: 140, new(P: 13, F: 6, C: 11 - 4)));
    public static Food NutritionalYeast_WalMart_8_Grams { get; } = new(
        "nutritional yeast from WalMart", new(
            // 2, ServingUnits.Tablespoon,
            8, ServingUnits.Gram,
            Cals: 30,
            new(P: 4, F: 0, C: 3 - 2)));

    public static Food NutritionalYeast_Sprouts_16_Grams { get; } = new(
    "nutritional yeast from Sprouts", new(
        // 2, ServingUnits.Tablespoon,
        16, ServingUnits.Gram,
        Cals: 60,
        new(P: 5, F: 0.5, C: 5 - 3)));

    public static Food Oatmeal_1_2_Cup { get; } = new("oatmeal",
        new(0.5, ServingUnits.Cup, Cals: 140, new(P: 5, F: 2.5, C: 27 - 4)));
    public static Food OliveOil_1_Tbsp { get; } = new(
        "olive oil",
        new(1, ServingUnits.Tablespoon, Cals: 120, new(P: 0, F: 14, C: 0)));

    // https://shop.sprouts.com/product/55477/organic-raw-pumpkin-seeds
    public static Food PumpkinSeeds_30_Grams { get; } = new Food(
        "pumpkin seeds",
        new(30, ServingUnits.Gram, Cals: 170, new(P: 9, F: 15, C: 3 - 2)));

    // https://www.walmart.com/ip/Bob-s-Red-Mill-Organic-Farro-24-oz-Pkg/762388784?from=/search
    public static Food Farro_52_Gram { get; } = new Food(
        "farro", new(
            // 0.25, ServingUnits.Cup
            52, ServingUnits.Gram,
            Cals: 190,
            new(P: 6, F: 1, C: 38 - 5)));

    public static Food Gluten_30_Grams { get; } = new(
    "gluten", new(
        // 0.25, ServingUnits.Cup,
        30, ServingUnits.Gram,
        Cals: 120,
        new(P: 23, F: 1, C: 4 - 0)));

    public static Food Tofu_1_5_block { get; } = new(
        "tofu",
        new(0.2, ServingUnits.BlockTofu, Cals: 130, new(P: 14, F: 7, C: 0)));

    public static Food Whey_1_Scoop { get; } = new(
        "whey",
        new(1, ServingUnits.Scoop, Cals: 130, new(P: 30, F: 0.5, C: 1 - 0)));

    // Converted
    private static readonly double CUPS_PER_SCOOP = 0.380408;
    public static Food Edamame_1_Scoop { get; } = new(
        Edamame_1_4_Cup.Name, new(
            1, ServingUnits.Scoop,
            Edamame_1_4_Cup.NutritionalInformation.Cals * 4 * CUPS_PER_SCOOP,
            Edamame_1_4_Cup.NutritionalInformation.Macros * 4 * CUPS_PER_SCOOP));

    public static Food Seitan_Walmart_Yeast_1_Gram_Gluten_4x { get; } = new(
        "Seitan Walmart Nutritional Yeast, 4x gluten",
        NutritionalYeast_WalMart_8_Grams.NutritionalInformation.Combine(Gluten_30_Grams.NutritionalInformation, 4));
    public static Food Oatmeal_1_Scoop { get; } = new(
        Oatmeal_1_2_Cup.Name, new(
            1, ServingUnits.Scoop,
            Oatmeal_1_2_Cup.NutritionalInformation.Cals * 2 * CUPS_PER_SCOOP,
            Oatmeal_1_2_Cup.NutritionalInformation.Macros * 2 * CUPS_PER_SCOOP));
}

