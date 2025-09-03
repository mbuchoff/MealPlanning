using SystemOfEquations;

namespace SystemOfEquations.Data;

internal static class Foods
{
    // Base food definitions (private)
    private static Food AlmondButterFood { get; } = new("almond butter",
        [(1, ServingUnits.Tablespoon)],
        new BaseNutrition(Cals: 130, P: 4.5M, F: 11.5M, CTotal: 4.5M, CFiber: 2.5M));
    
    private static Food AlmondMilkFood { get; } = new("almond milk",
        [(2, ServingUnits.Cup), (480, ServingUnits.Gram)],
        new BaseNutrition(Cals: 59, P: 2, F: 5.1M, CTotal: 2, CFiber: 2));
    
    private static Food AppleFood { get; } = new("apple",
        [(1, ServingUnits.None)],
        new BaseNutrition(Cals: 95, P: 1, F: 0, CTotal: 25, CFiber: 3));
    
    private static Food BlackBeansFood { get; } = new("black beans",
        [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 150, P: 10, F: 0.5M, CTotal: 28, CFiber: 7));
    
    private static Food BlueBerries { get; } = new("frozen blueberries",
        [(1, ServingUnits.Cup), (140, ServingUnits.Gram)],
        new BaseNutrition(Cals: 70, P: 0, F: 1, CTotal: 17, CFiber: 4));
    
    private static Food BrownRiceFood { get; } = new("brown rice",
        [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2),
        Water: new(Base: 1.5M, PerServing: 0.5M));
    
    private static Food ChiaSeedsFood { get; } = new("chia seeds",
        [(2.5M, ServingUnits.Tablespoon)],
        new BaseNutrition(Cals: 150, P: 5, F: 9, CTotal: 13, CFiber: 10));
    
    private static Food CreatineFood { get; } = new("creatine",
        [(1, ServingUnits.Scoop)],
        new BaseNutrition(Cals: 0, P: 0, F: 0, CTotal: 0, CFiber: 0));
    
    private static Food EzekielCerealFood { get; } = new("Ezekiel cereal",
        [(0.5M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 180, P: 8, F: 1, CTotal: 35, CFiber: 6));
    
    private static Food EzekielCerealLowSodiumFood { get; } = new("Ezekiel cereal, low sodium",
        [(0.5M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 190, P: 8, F: 1, CTotal: 38, CFiber: 7));
    
    private static Food EzekielEnglishMuffinFood { get; } = new("Ezekiel english muffin",
        [(1, ServingUnits.None)],
        new BaseNutrition(Cals: 90, P: 6, F: 0.5M, CTotal: 17, CFiber: 3));
    
    private static Food NutritionalYeastFood { get; } = new("nutritional yeast from Sprouts",
        [(16, ServingUnits.Gram), (2, ServingUnits.Tablespoon)],
        new BaseNutrition(Cals: 60, P: 5, F: 0.5M, CTotal: 5, CFiber: 3));
    
    private static Food OatmealWalmartFood { get; } = new("oatmeal from WalMart",
        [(0.5M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 140, P: 5, F: 2.5M, CTotal: 27, CFiber: 4));
    
    private static Food OatsFood { get; } = new("oats",
        [(0.5M, ServingUnits.Cup), (40, ServingUnits.Gram)],
        new BaseNutrition(Cals: 170, P: 6, F: 3, CTotal: 30, CFiber: 5));
    
    private static Food OliveOilFood { get; } = new("olive oil",
        [(1, ServingUnits.Tablespoon)],
        new BaseNutrition(Cals: 120, P: 0, F: 14, CTotal: 0, CFiber: 0));
    
    private static Food OrangeJuiceFood { get; } = new("orange juice",
        [(1, ServingUnits.Cup)],
        new BaseNutrition(Cals: 110, P: 2, F: 0, CTotal: 27, CFiber: 0));
    
    private static Food PumpkinSeedsFood { get; } = new("pumpkin seeds",
        [(30, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));
    
    private static Food FarroFood { get; } = new("farro",
        [(52, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 190, P: 6, F: 1, CTotal: 38, CFiber: 5),
        Water: new(Base: 0, PerServing: 1.333333333332M));
    
    // Conversion foods
    private static Food FatToCarbConversionFood { get; } = new("fat to carb conversion",
        [(1, ServingUnits.Gram)],
        new BaseNutrition(Cals: 0, P: 0, F: 1, CTotal: -9M / 4M, CFiber: 0),
        IsConversion: true);
    
    private static Food FatToProteinConversionFood { get; } = new("fat to protein conversion",
        [(1, ServingUnits.Gram)],
        new BaseNutrition(Cals: 0, P: -9M / 4M, F: 1, CTotal: 0, CFiber: 0),
        IsConversion: true);
    
    private static Food ProteinToCarbConversionFood { get; } = new("protein to carb conversion",
        [(1, ServingUnits.Gram)],
        new BaseNutrition(Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0),
        IsConversion: true);
    
    private static Food ProteinToFatConversionFood { get; } = new("protein to fat conversion",
        [(1, ServingUnits.Gram)],
        new BaseNutrition(Cals: 9, P: -9M / 4M, F: 1, CTotal: 0, CFiber: 0),
        IsConversion: true);
    
    private static Food GlutenFood { get; } = new("gluten",
        [(30, ServingUnits.Gram), (0.25M, ServingUnits.Cup), (4, ServingUnits.Tablespoon)],
        new BaseNutrition(Cals: 120, P: 23, F: 1, CTotal: 4, CFiber: 0));
    
    private static Food PeaProteinFood { get; } = new("pea protein",
        [(1M / 3, ServingUnits.Cup), (33, ServingUnits.Gram)],
        new BaseNutrition(Cals: 110, P: 24, F: 1.5M, CTotal: 1, CFiber: 0));
    
    private static Food PearledBarleyFood { get; } = new("pearled barley",
        [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 160, P: 4, F: 0.5M, CTotal: 35, CFiber: 7),
        Water: new(Base: 1.5M, PerServing: 0.6M));
    
    private static Food QuinoaFood { get; } = new("quinoa",
        [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 170, P: 6, F: 2.5M, CTotal: 29, CFiber: 3));
    
    private static Food RedKidneyBeansFood { get; } = new("red kidney beans",
        [(0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 150, P: 10, F: 0, CTotal: 28, CFiber: 7));
    
    private static Food RedLentilsFood { get; } = new("red lentils",
        [(0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 170, P: 11, F: 1, CTotal: 30, CFiber: 5));
    
    private static Food SunflowerSeedsFood { get; } = new("Sunflower seeds",
        [(30, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 180, P: 6, F: 15, CTotal: 6, CFiber: 3));
    
    private static Food ToastedWheatfulsFood { get; } = new("toasted wheatfuls",
        [(50, ServingUnits.Gram)],
        new BaseNutrition(Cals: 200, P: 7, F: 1.5M, CTotal: 48, CFiber: 8));
    
    private static Food TofuFood { get; } = new("tofu",
        [(91, ServingUnits.Gram)],
        new BaseNutrition(Cals: 130, P: 14, F: 7, CTotal: 2, CFiber: 2));
    
    private static Food WheatBerriesFood { get; } = new("wheat berries",
        [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 150, P: 7, F: 1, CTotal: 31, CFiber: 5),
        Water: new(Base: 2, PerServing: 0.6M));
    
    private static Food WheatBranFood { get; } = new("wheat bran",
        [(0.5M, ServingUnits.Cup)],
        new BaseNutrition(Cals: 63, P: 4.5M, F: 1.3M, CTotal: 18.5M, CFiber: 12.5M));
    
    private static Food WholGrainPastaFood { get; } = new("Pasta",
        [(56, ServingUnits.Gram)],
        new BaseNutrition(Cals: 180, P: 8, F: 1.5M, CTotal: 39, CFiber: 7));
    
    private static Food EdamameFood { get; } = new("edamame",
        [(1M / 3, ServingUnits.Cup)],
        new BaseNutrition(Cals: 130, P: 14, F: 5, CTotal: 9, CFiber: 6));

    // Public FoodServing definitions
    public static FoodServing AlmondButter_1_Tbsp => 
        AlmondButterFood.WithServing(1, ServingUnits.Tablespoon);

    public static FoodServing AlmondMilk_2_Cup => 
        AlmondMilkFood.WithServing(2, ServingUnits.Cup);

    public static FoodServing Apple => 
        AppleFood.WithServing(1, ServingUnits.None);

    // https://shop.sprouts.com/product/7647/black-beans
    public static FoodServing BlackBeans_Sprouts_45g => 
        BlackBeansFood.WithServing(45, ServingUnits.Gram);

    public static FoodServing BlueBerries_1_Cup => 
        BlueBerries.WithServing(1, ServingUnits.Cup);

    public static FoodServing BrownRice_45_Grams => 
        BrownRiceFood.WithServing(45, ServingUnits.Gram);

    public static FoodServing ChiaSeeds_2_5_Tbsp => 
        ChiaSeedsFood.WithServing(2.5M, ServingUnits.Tablespoon);

    public static FoodServing Creatine_1_Scoop => 
        CreatineFood.WithServing(1, ServingUnits.Scoop);

    public static FoodServing Ezeliel_Cereal_1_2_Cup => 
        EzekielCerealFood.WithServing(0.5M, ServingUnits.Cup);

    public static FoodServing Ezeliel_Cereal_Low_Sodium_1_2_Cup => 
        EzekielCerealLowSodiumFood.WithServing(0.5M, ServingUnits.Cup);

    public static FoodServing Ezekiel_English_Muffin => 
        EzekielEnglishMuffinFood.WithServing(1, ServingUnits.None);

    public static FoodServing NutritionalYeast_Sprouts_16_Grams => 
        NutritionalYeastFood.WithServing(16, ServingUnits.Gram);

    public static FoodServing Oatmeal_Walmart_1_2_Cup => 
        OatmealWalmartFood.WithServing(0.5M, ServingUnits.Cup);

    // https://shop.sprouts.com/product/7931/rolled-oats
    public static FoodServing Oats_1_2_Cup => 
        OatsFood.WithServing(0.5M, ServingUnits.Cup);

    public static FoodServing OliveOil_1_Tbsp => 
        OliveOilFood.WithServing(1, ServingUnits.Tablespoon);

    // https://www.walmart.com/ip/Florida-s-Natural-Orange-Juice-With-Pulp-89-oz/101698030
    public static FoodServing OrangeJuice_1_Cup => 
        OrangeJuiceFood.WithServing(1, ServingUnits.Cup);

    // https://shop.sprouts.com/product/55477/organic-raw-pumpkin-seeds
    public static FoodServing PumpkinSeeds_30_Grams => 
        PumpkinSeedsFood.WithServing(30, ServingUnits.Gram);

    // https://www.walmart.com/ip/Bob-s-Red-Mill-Organic-Farro-24-oz-Pkg/762388784?from=/search
    public static FoodServing Farro_52_Gram => 
        FarroFood.WithServing(52, ServingUnits.Gram);

    public static FoodServing FatToCarbConversion => 
        FatToCarbConversionFood.WithServing(1, ServingUnits.Gram);

    public static FoodServing FatToProteinConversion => 
        FatToProteinConversionFood.WithServing(1, ServingUnits.Gram);

    public static FoodServing ProteinToCarbConversion => 
        ProteinToCarbConversionFood.WithServing(1, ServingUnits.Gram);

    public static FoodServing ProteinToFatConversion => 
        ProteinToFatConversionFood.WithServing(1, ServingUnits.Gram);

    public static FoodServing Gluten_30_Grams => 
        GlutenFood.WithServing(30, ServingUnits.Gram);

    public static FoodServing Gluten_4_Tbsp => 
        GlutenFood.WithServing(4, ServingUnits.Tablespoon);

    // https://shop.sprouts.com/product/57875/pea-isolate-80percent-protein-powder
    public static FoodServing PeaProtein_1_3_Cup => 
        PeaProteinFood.WithServing(1M / 3, ServingUnits.Cup);

    // https://shop.sprouts.com/product/7827/pearled-barley
    public static FoodServing PearledBarley_45_Grams => 
        PearledBarleyFood.WithServing(45, ServingUnits.Gram);

    // https://shop.sprouts.com/store/sprouts/products/17847854-organic-tri-color-quinoa-bulk-1-lb
    public static FoodServing Quinoa_45_Grams => 
        QuinoaFood.WithServing(45, ServingUnits.Gram);

    // https://shop.sprouts.com/product/7654/organic-dark-red-kidney-beans
    public static FoodServing RedKidneyBeans_1_4_Cup => 
        RedKidneyBeansFood.WithServing(0.25M, ServingUnits.Cup);

    // https://shop.sprouts.com/product/7706/organic-split-red-lentils
    public static FoodServing RedLentils_1_4_Cup => 
        RedLentilsFood.WithServing(0.25M, ServingUnits.Cup);

    // https://shop.sprouts.com/product/55813/organic-raw-hulled-sunflower-seeds
    public static FoodServing SunflowerSeeds_30_Grams => 
        SunflowerSeedsFood.WithServing(30, ServingUnits.Gram);

    public static FoodServing ToastedWheatfuls => 
        ToastedWheatfulsFood.WithServing(50, ServingUnits.Gram);

    public static FoodServing Tofu_91_Grams => 
        TofuFood.WithServing(91, ServingUnits.Gram);

    // https://shop.sprouts.com/product/22564/hard-spring-wheat-berries
    public static FoodServing WheatBerries_45_Grams => 
        WheatBerriesFood.WithServing(45, ServingUnits.Gram);

    // https://shop.sprouts.com/product/7847/wheat-bran
    // https://www.healthline.com/nutrition/wheat-bran#nutrition
    public static FoodServing WheatBran_1_2_Cup => 
        WheatBranFood.WithServing(0.5M, ServingUnits.Cup);

    public static FoodServing BlueBerries_1_Scoop => 
        BlueBerries.WithServing(1, ServingUnits.Scoop);

    public static FoodServing Seitan_Sprouts_Yeast_1_Gram_Gluten_4x => new(
        "Seitan Walmart Nutritional Yeast, 4x gluten",
        NutritionalYeast_Sprouts_16_Grams.NutritionalInformation.Combine(Gluten_30_Grams.NutritionalInformation, 4),
        Water: new(Base: 0, PerServing: 0.0366666666666667M));

    public static FoodServing Whole_Grain_Pasta_56_Grams => 
        WholGrainPastaFood.WithServing(56, ServingUnits.Gram);

    public static FoodServing Edamame_1_Scoop => 
        EdamameFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing AlmondMilk_1_Scoop => 
        AlmondMilkFood.WithServing(1, ServingUnits.Scoop);

    public static FoodServing Ezeliel_Cereal_Low_Sodium_1_Scoop => 
        EzekielCerealLowSodiumFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing Oats_1_Scoop => 
        OatsFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing PeaProtein_1_Scoop => 
        PeaProteinFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing PumpkinSeeds_1_Scoop => 
        PumpkinSeedsFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing WheatBran_1_Scoop => 
        WheatBranFood.WithServing(1, ServingUnits.Scoop);
    public static FoodServing PeaProtein_1_Tbsp => 
        PeaProteinFood.WithServing(1, ServingUnits.Tablespoon);
}
