using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data;

internal static class FoodGroupings
{
    public static FoodGrouping ApplesBlueberriesOatmealAndEdamame { get; } = new(
        "apples, blueberries, oatmeal, and edamame",
        [
            new(Foods.Apple, ServingUnits.Apple, Servings: 1),
            new(Foods.BlueBerries_1_Scoop, ServingUnits.Scoop, Servings: 2),
        ],
        Foods.Edamame_1_Scoop, ServingUnits.Scoop,
        Foods.ChiaSeeds_2_5_Tbsp, ServingUnits.Tablespoon,
        Foods.Oats_1_Scoop, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Ezekial(bool withEdamame = true) => new(
        "Ezekial",
        [new(Foods.AlmondMilk_2_Cup, ServingUnits.Cup, Servings: 0.5)],
        Foods.PumpkinSeeds, ServingUnits.Scoop,
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToFatConversion, ServingUnits.Scoop,
        Foods.Ezeliel_1_2_Cup, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping OatmealWithAlmondButter { get; } = new(
        "oatmeal with almond butter",
        Foods.Oats_1_Scoop, ServingUnits.Scoop,
        Foods.AlmondButter_1_Tbsp, ServingUnits.Tablespoon,
        Foods.WheatBran_1_Scoop, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueBerryOatmeal { get; } = new(
        "blueberry oatmeal",
        [new(Foods.BlueBerries_1_Scoop, ServingUnits.Scoop, Servings: 3)],
        Foods.WheatBran_1_Scoop, ServingUnits.Scoop,
        Foods.ChiaSeeds_2_5_Tbsp, ServingUnits.Tablespoon,
        Foods.Oats_1_Scoop, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping OatmealAndEdamame { get; } = new(
        "Oatmeal and edamame",
        Foods.Edamame_1_Scoop, ServingUnits.Scoop,
        Foods.AlmondButter_1_Tbsp, ServingUnits.Tablespoon,
        Foods.Oats_1_Scoop, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping ProteinShake { get; } = new(
        "protein shake",
        [new(Foods.AlmondMilk_2_Cup, ServingUnits.Cup, Servings: 1)],
        Foods.PeaProtein_1_Scoop, ServingUnits.Scoop,
        Foods.ChiaSeeds_2_5_Tbsp, ServingUnits.Tablespoon,
        Foods.BlueBerries_1_Scoop, ServingUnits.Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Rice_BlackBeans_OliveOil { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g, ServingUnits.Gram,
        Foods.OliveOil_1_Tbsp, ServingUnits.Tablespoon,
        Foods.BrownRice_45_Grams, ServingUnits.Gram,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Rice_BlackBeans_PumpkinSeeds { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g, ServingUnits.Gram,
        Foods.PumpkinSeeds, ServingUnits.Gram,
        Foods.BrownRice_45_Grams, ServingUnits.Gram,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarley_BlackBeans_PumpkinSeeds { get; } = new(
        "pearled barley and beans",
        Foods.BlackBeans_Sprouts_45g, ServingUnits.Gram,
        Foods.PumpkinSeeds, ServingUnits.Gram,
        Foods.PearledBarley_45_Grams, ServingUnits.Gram,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Seitan { get; } = new(
        "seitan",
        Foods.Seitan_Sprouts_Yeast_1_Gram_Gluten_4x, ServingUnits.Gram,
        Foods.OliveOil_1_Tbsp, ServingUnits.Tablespoon,
        Foods.WheatBerries_45_Grams, ServingUnits.Gram,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Tofu { get; } = new(
        "tofu",
        Foods.Tofu_1_5_Block, ServingUnits.BlockTofu,
        Foods.PumpkinSeeds, ServingUnits.Gram,
        Foods.Farro_52_Gram, ServingUnits.Gram,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping WorkoutShake { get; } = new(
        "workout shake",
        Foods.PeaProtein_1_Scoop, ServingUnits.Scoop,
        Foods.FatToCarbConversion, ServingUnits.Gram,
        Foods.OrangeJuice_1_Cup, ServingUnits.Cup,
        PreparationMethodEnum.PrepareAsNeeded);
}
