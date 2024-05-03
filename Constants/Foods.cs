namespace SystemOfEquations.Constants;

internal static class Foods
{
    public static Food AlmondButter_1_Tbsp { get; } = new("almond butter", new(
        1, ServingUnits.Tablespoon, Cals: 130, P: 4.5, F: 11.5, CTotal: 4.5, CFiber: 2.5));
    public static Food AlmondMilk_2_Cup { get; } = new("almond milk", new(
        2, ServingUnits.Cup, Cals: 59, P: 2, F: 5.1, CTotal: 2, CFiber: 2));
    public static Food BlueBerries_1_Cup { get; } = new("frozen blueberries", new(
        1, ServingUnits.Cup, Cals: 80, P: 0, F: 0, CTotal: 19, CFiber: 6));
    public static Food BrownRice_45_Grams { get; } = new("brown rice", new(
        // 0.25, ServingUnits.Cup
        45, ServingUnits.Gram, Cals: 170, P: 4, F: 1.5, CTotal: 35, CFiber: 2),
        CupsWaterPerServing: 0.770171149143);
    public static Food Cassein_1_Scoop { get; } = new("cassein", new(
        1, ServingUnits.Scoop, Cals: 100, P: 25, F: 0, CTotal: 1, CFiber: 0));
    public static Food ChiaSeeds_2_5_Tbsp { get; } = new("chia seeds", new(
        2.5, ServingUnits.Tablespoon, Cals: 150, P: 5, F: 9, CTotal: 13, CFiber: 10));
    public static Food Edamame_1_4_Cup { get; } = new("edamame", new(
        0.25, ServingUnits.Cup, Cals: 140, P: 13, F: 6, CTotal: 11, CFiber: 4));
    public static Food Edamame_35_Grams { get; } = new(Edamame_1_4_Cup.Name, new(
        35, ServingUnits.Gram,
        Edamame_1_4_Cup.NutritionalInformation.Cals,
        Edamame_1_4_Cup.NutritionalInformation.P,
        Edamame_1_4_Cup.NutritionalInformation.F,
        Edamame_1_4_Cup.NutritionalInformation.CTotal,
        Edamame_1_4_Cup.NutritionalInformation.CFiber));
    public static Food NutritionalYeast_WalMart_8_Grams { get; } = new("nutritional yeast from WalMart", new(
        // 2, ServingUnits.Tablespoon,
        8, ServingUnits.Gram,
        Cals: 30,
        P: 4, F: 0, CTotal: 3, CFiber: 2));

    public static Food NutritionalYeast_Sprouts_16_Grams { get; } = new("nutritional yeast from Sprouts", new(
        // 2, ServingUnits.Tablespoon,
        16, ServingUnits.Gram,
        Cals: 60,
        P: 5, F: 0.5, CTotal: 5, CFiber: 3));

    public static Food Oatmeal_Walmart_1_2_Cup { get; } = new("oatmeal from WalMart", new(
        0.5, ServingUnits.Cup, Cals: 140, P: 5, F: 2.5, CTotal: 27, CFiber: 4));

    // https://shop.sprouts.com/product/7931/rolled-oats
    public static Food Oatmeal_Sprouts_1_2_Cup { get; } = new("oatmeal from Sprouts", new(
        0.5, ServingUnits.Cup, Cals: 170, P: 6, F: 3, CTotal: 30, CFiber: 5));

    public static Food OliveOil_1_Tbsp { get; } = new("olive oil", new(
        1, ServingUnits.Tablespoon, Cals: 120, P: 0, F: 14, CTotal: 0, CFiber: 0));

    // https://www.walmart.com/ip/Florida-s-Natural-Orange-Juice-With-Pulp-89-oz/101698030
    public static Food OrangeJuice_1_Cup { get; } = new("orange juice", new(
        1, ServingUnits.Cup, Cals: 110, P: 2, F: 0, CTotal: 27, CFiber: 0));

    // https://shop.sprouts.com/product/55477/organic-raw-pumpkin-seeds
    public static Food PumpkinSeeds_30_Grams { get; } = new Food("pumpkin seeds", new(
        30, ServingUnits.Gram, Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));

    // https://www.walmart.com/ip/Bob-s-Red-Mill-Organic-Farro-24-oz-Pkg/762388784?from=/search
    public static Food Farro_52_Gram { get; } = new Food("farro", new(
        // 0.25, ServingUnits.Cup
        52, ServingUnits.Gram,
        Cals: 190,
        P: 6, F: 1, CTotal: 38, CFiber: 5),
        CupsWaterPerServing: 1.333333333332);

    public static Food FatToCarbConversion { get; } = new Food("fat to carb conversion", new(
        1, ServingUnits.Gram, Cals: 9, P: 0, F: 1, CTotal: -9.0 / 4.0, CFiber: 0), Hidden: true);

    public static Food Gluten_30_Grams { get; } = new("gluten", new(
        // 0.25, ServingUnits.Cup,
        30, ServingUnits.Gram,
        Cals: 120,
        P: 23, F: 1, CTotal: 4, CFiber: 0));

    // https://shop.sprouts.com/product/57875/pea-isolate-80percent-protein-powder
    public static Food PeaProtein_1_3_Cup { get; } = new("pea protein", new(
        1.0 / 3, ServingUnits.Cup, Cals: 110, P: 24, F: 1.5, CTotal: 1, CFiber: 0));

    // https://shop.sprouts.com/product/7654/organic-dark-red-kidney-beans
    public static Food RedKidneyBeans_1_4_Cup { get; } = new("red kidney beans", new(
        1.0 / 4, ServingUnits.Cup, Cals: 150, P: 10, F: 0, CTotal: 28, CFiber: 7));

    // https://shop.sprouts.com/product/7706/organic-split-red-lentils
    public static Food RedLentils_1_4_Cup { get; } = new("red lentils", new(
        1.0 / 4, ServingUnits.Cup, Cals: 170, P: 11, F: 1, CTotal: 30, CFiber: 5));

    // https://shop.sprouts.com/product/22870/10-bean-soup-mix
    public static Food Sprouts_10_Bean_Soup_Mix_35_Grams { get; } = new("Sprouts 10-bean soup mix", new(
        35, ServingUnits.Gram, Cals: 120, P: 8, F: 0, CTotal: 22, CFiber: 5));

    // https://shop.sprouts.com/product/55813/organic-raw-hulled-sunflower-seeds
    public static Food SunflowerSeeds_30_Grams { get; } = new("Sunflower seeds", new(
        30, ServingUnits.Gram, Cals: 180, P: 6, F: 15, CTotal: 6, CFiber: 3));

    public static Food Tofu_1_5_Block { get; } = new("tofu", new(
        0.2, ServingUnits.BlockTofu, Cals: 130, P: 14, F: 7, CTotal: 2, CFiber: 2));

    // https://shop.sprouts.com/product/22564/hard-spring-wheat-berries
    public static Food WheatBerries_45_Grams { get; } = new("wheat berries", new(
        45, ServingUnits.Gram, Cals: 150, P: 7, F: 1, CTotal: 31, CFiber: 5), CupsWaterPerServing: 0.669144981411);

    public static Food Whey_1_Scoop { get; } = new("whey", new(
        1, ServingUnits.Scoop, Cals: 130, P: 30, F: 0.5, CTotal: 1, CFiber: 0));

    // Converted
    private static readonly double CUPS_PER_SCOOP = 0.380408;
    public static readonly double TBSP_PER_CUP = 16;

    public static Food Blueberries_1_Scoop { get; } = BlueBerries_1_Cup.Convert(1, ServingUnits.Scoop, CUPS_PER_SCOOP);

    public static Food Edamame_1_Scoop { get; } = Edamame_1_4_Cup.Convert(1, ServingUnits.Scoop, 4 * CUPS_PER_SCOOP);

    public static Food Seitan_Walmart_Yeast_1_Gram_Gluten_4x { get; } = new(
        "Seitan Walmart Nutritional Yeast, 4x gluten",
        NutritionalYeast_WalMart_8_Grams.NutritionalInformation.Combine(Gluten_30_Grams.NutritionalInformation, 4));

    public static Food Oatmeal_Sprouts_1_Scoop { get; } = Oatmeal_Sprouts_1_2_Cup.Convert(1, ServingUnits.Scoop, 2 * CUPS_PER_SCOOP);
    public static Food Oatmeal_Walmart_1_Scoop { get; } = Oatmeal_Walmart_1_2_Cup.Convert(1, ServingUnits.Scoop, 2 * CUPS_PER_SCOOP);
    public static Food PeaProtein_1_Scoop { get; } = PeaProtein_1_3_Cup.Convert(1, ServingUnits.Scoop, 3 * CUPS_PER_SCOOP);
    public static Food PeaProtein_1_Tbsp { get; } = PeaProtein_1_3_Cup.Convert(1, ServingUnits.Tablespoon, 3 / TBSP_PER_CUP);
}

