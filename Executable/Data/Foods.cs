﻿namespace SystemOfEquations.Data;

internal static class Foods
{
    public static Food AlmondButter_1_Tbsp { get; } = new("almond butter", new(
        1, ServingUnits.Tablespoon, Cals: 130, P: 4.5M, F: 11.5M, CTotal: 4.5M, CFiber: 2.5M));

    public static Food AlmondMilk_2_Cup { get; } = new("almond milk", new(
        2, ServingUnits.Cup, Cals: 59, P: 2, F: 5.1M, CTotal: 2, CFiber: 2));

    public static Food Apple { get; } = new("apple", new(
        1, ServingUnits.None, Cals: 95, P: 1, F: 0, CTotal: 25, CFiber: 3));

    // https://shop.sprouts.com/product/7647/black-beans
    public static Food BlackBeans_Sprouts_45g { get; } = new("black beans", new(
        45, ServingUnits.Gram, Cals: 150, P: 10, F: 0.5M, CTotal: 28, CFiber: 7));

    public static Food BlueBerries_1_Cup { get; } = new("frozen blueberries", new(
        // 140, ServingUnits.Grams
        1, ServingUnits.Cup, Cals: 70, P: 0, F: 1, CTotal: 17, CFiber: 4));

    public static Food BrownRice_45_Grams { get; } = new("brown rice", new(
        // 0.25, ServingUnits.Cup
        45, ServingUnits.Gram, Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2),
        Water: new(Base: 1.5M, PerServing: 0.5M));

    public static Food ChiaSeeds_2_5_Tbsp { get; } = new("chia seeds", new(
        2.5M, ServingUnits.Tablespoon, Cals: 150, P: 5, F: 9, CTotal: 13, CFiber: 10));

    public static Food Ezeliel_Cereal_1_2_Cup { get; } = new("Ezekiel cereal", new(
        0.5M, ServingUnits.Cup, Cals: 180, P: 8, F: 1, CTotal: 35, CFiber: 6));

    public static Food Ezeliel_Cereal_Low_Sodium_1_2_Cup { get; } = new("Ezekiel cereal, low sodium", new(
        0.5M, ServingUnits.Cup, Cals: 190, P: 8, F: 1, CTotal: 38, CFiber: 7));

    public static Food Ezekiel_English_Muffin { get; } = new("Ezekiel english muffin", new(
        1, ServingUnits.None, Cals: 90, P: 6, F: 0.5M, CTotal: 17, CFiber: 3));

    public static Food NutritionalYeast_Sprouts_16_Grams { get; } = new("nutritional yeast from Sprouts", new(
        // 2, ServingUnits.Tablespoon,
        16, ServingUnits.Gram,
        Cals: 60,
        P: 5, F: 0.5M, CTotal: 5, CFiber: 3));

    public static Food Oatmeal_Walmart_1_2_Cup { get; } = new("oatmeal from WalMart", new(
        0.5M, ServingUnits.Cup, Cals: 140, P: 5, F: 2.5M, CTotal: 27, CFiber: 4));

    // https://shop.sprouts.com/product/7931/rolled-oats
    public static Food Oats_1_2_Cup { get; } = new("oats", new(
        0.5M, ServingUnits.Cup, Cals: 170, P: 6, F: 3, CTotal: 30, CFiber: 5));

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
        Water: new(Base: 0, PerServing: 1.333333333332M));

    public static Food FatToCarbConversion { get; } = new Food("fat to carb conversion", new(
        1, ServingUnits.Gram, Cals: 0, P: 0, F: 1, CTotal: -9M / 4M, CFiber: 0), IsConversion: true);

    public static Food FatToProteinConversion { get; } = new Food("fat to protein conversion", new(
        1, ServingUnits.Gram, Cals: 0, P: -9M / 4M, F: 1, CTotal: 0, CFiber: 0), IsConversion: true);

    public static Food ProteinToCarbConversion { get; } = new Food("protein to carb conversion", new(
        1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0), IsConversion: true);

    public static Food ProteinToFatConversion { get; } = new Food("protein to fat conversion", new(
        1, ServingUnits.Gram, Cals: 9, P: -9M / 4M, F: 1, CTotal: 0, CFiber: 0), IsConversion: true);

    public static Food Gluten_30_Grams { get; } = new("gluten", new(
        // 0.25, ServingUnits.Cup,
        30, ServingUnits.Gram,
        Cals: 120,
        P: 23, F: 1, CTotal: 4, CFiber: 0));

    public static Food Gluten_4_Tbsp { get; } = Gluten_30_Grams.Copy(ServingUnits.Tablespoon, 4);

    // https://shop.sprouts.com/product/57875/pea-isolate-80percent-protein-powder
    public static Food PeaProtein_1_3_Cup { get; } = new("pea protein", new(
        1M / 3, ServingUnits.Cup, Cals: 110, P: 24, F: 1.5M, CTotal: 1, CFiber: 0));

    // https://shop.sprouts.com/product/7827/pearled-barley
    public static Food PearledBarley_45_Grams { get; } = new("pearled barley", new(
        // 0.25 cups
        45, ServingUnits.Gram, Cals: 160, P: 4, F: 0.5M, CTotal: 35, CFiber: 7),
        Water: new(Base: 1.5M, PerServing: 0.6M));

    // https://shop.sprouts.com/store/sprouts/products/17847854-organic-tri-color-quinoa-bulk-1-lb
    public static Food Quinoa_45_Grams { get; } = new("quinoa",
        new(45, ServingUnits.Gram, Cals: 170, P: 6, F: 2.5M, CTotal: 29, CFiber: 3));

    // https://shop.sprouts.com/product/7654/organic-dark-red-kidney-beans
    public static Food RedKidneyBeans_1_4_Cup { get; } = new("red kidney beans",
        new(1M / 4, ServingUnits.Cup, Cals: 150, P: 10, F: 0, CTotal: 28, CFiber: 7));

    // https://shop.sprouts.com/product/7706/organic-split-red-lentils
    public static Food RedLentils_1_4_Cup { get; } = new("red lentils",
        new(1M / 4, ServingUnits.Cup, Cals: 170, P: 11, F: 1, CTotal: 30, CFiber: 5));

    // https://shop.sprouts.com/product/55813/organic-raw-hulled-sunflower-seeds
    public static Food SunflowerSeeds_30_Grams { get; } = new("Sunflower seeds",
        new(30, ServingUnits.Gram, Cals: 180, P: 6, F: 15, CTotal: 6, CFiber: 3));

    public static Food ToastedWheatfuls { get; } = new("toasted wheatfuls",
        new(50, ServingUnits.Gram, Cals: 200, P: 7, F: 1.5M, CTotal: 48, CFiber: 8));

    public static Food Tofu_91_Grams { get; } = new("tofu",
        new(91, ServingUnits.Gram, Cals: 130, P: 14, F: 7, CTotal: 2, CFiber: 2));

    // https://shop.sprouts.com/product/22564/hard-spring-wheat-berries
    public static Food WheatBerries_45_Grams { get; } = new("wheat berries",
        new(45, ServingUnits.Gram, Cals: 150, P: 7, F: 1, CTotal: 31, CFiber: 5),
        Water: new(Base: 2, PerServing: 1.4M));

    // https://shop.sprouts.com/product/7847/wheat-bran
    // https://www.healthline.com/nutrition/wheat-bran#nutrition
    public static Food WheatBran_1_2_Cup { get; } = new("wheat bran",
        new(0.5M, ServingUnits.Cup, Cals: 63, P: 4.5M, F: 1.3M, CTotal: 18.5M, CFiber: 12.5M));

    public static Food BlueBerries_1_Scoop { get; } = BlueBerries_1_Cup.Convert(ServingUnits.Scoop);

    public static Food Seitan_Sprouts_Yeast_1_Gram_Gluten_4x { get; } = new(
        "Seitan Walmart Nutritional Yeast, 4x gluten",
        NutritionalYeast_Sprouts_16_Grams.NutritionalInformation.Combine(Gluten_30_Grams.NutritionalInformation, 4),
        Water: new(Base: 0, PerServing: 0.0366666666666667M));

    public static Food Whole_Grain_Pasta_56_Grams { get; } = new(
        "Pasta", new(56, ServingUnits.Gram, Cals: 180, P: 8, F: 1.5M, CTotal: 39, CFiber: 7));

    public static Food Edamame_1_Scoop { get; } = new Food("edamame",
        new(1M / 3, ServingUnits.Cup, Cals: 130, P: 14, F: 5, CTotal: 9, CFiber: 6)).Convert(ServingUnits.Scoop);
    public static Food AlmondMilk_1_Scoop { get; } =
        AlmondMilk_2_Cup.Convert(ServingUnits.Scoop);

    public static Food Ezeliel_Cereal_Low_Sodium_1_Scoop { get; } =
        Ezeliel_Cereal_Low_Sodium_1_2_Cup.Convert(ServingUnits.Scoop);
    public static Food Oats_1_Scoop { get; } = Oats_1_2_Cup.Convert(ServingUnits.Scoop);
    public static Food PeaProtein_1_Scoop { get; } = PeaProtein_1_3_Cup.Convert(ServingUnits.Scoop);
    public static Food PumpkinSeeds_1_Scoop { get; } = PumpkinSeeds_30_Grams
        .Copy(ServingUnits.Cup, newServings: 0.25M).Convert(ServingUnits.Scoop);
    public static Food WheatBran_1_Scoop { get; } = WheatBran_1_2_Cup.Convert(ServingUnits.Scoop);
    public static Food PeaProtein_1_Tbsp { get; } = PeaProtein_1_3_Cup.Convert(ServingUnits.Tablespoon);
}
