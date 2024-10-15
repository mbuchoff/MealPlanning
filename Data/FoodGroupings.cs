namespace SystemOfEquations.Data;

internal static class FoodGroupings
{
    public static FoodGrouping ApplesBlueberriesOatmealAndEdamame { get; } = new(
    "apples, blueberries, oatmeal, and edamame",
    [new(Foods.Apple, Servings: 2), new(Foods.BlueBerries_1_Scoop, Servings: 2)],
    Foods.Edamame_1_Scoop,
    Foods.ChiaSeeds_2_5_Tbsp,
    Foods.Oatmeal_Sprouts_1_Scoop,
    FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);


    public static FoodGrouping OatmealWithAlmondButter { get; } = new(
        "oatmeal with almond butter",
        Foods.Oatmeal_Sprouts_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        Foods.WheatBran_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueBerryOatmeal { get; } = new(
        "blueberry oatmeal",
        [new(Foods.BlueBerries_1_Scoop, Servings: 3)],
        Foods.WheatBran_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oatmeal_Sprouts_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueBerryOatmealAndEdamame { get; } = new(
        "blueberry oatmeal and edamame",
        [new(Foods.BlueBerries_1_Scoop, Servings: 3)],
        Foods.Edamame_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oatmeal_Sprouts_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping ProteinShake { get; } = new(
        "protein shake",
        [new(Foods.AlmondMilk_2_Cup, Servings: 1)],
        Foods.PeaProtein_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.BlueBerries_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Rice_BlackBeans_OliveOil { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Rice_BlackBeans_PumpkinSeeds { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.PumpkinSeeds_30_Grams,
        Foods.BrownRice_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarley_BlackBeans_OliveOil { get; } = new(
        "pearled barley and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.OliveOil_1_Tbsp,
        Foods.PearledBarley_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarley_BlackBeans_PumpkinSeeds { get; } = new(
        "pearled barley and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.PumpkinSeeds_30_Grams,
        Foods.PearledBarley_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Seitan { get; } = new(
        "seitan",
        Foods.Seitan_Sprouts_Yeast_1_Gram_Gluten_4x,
        Foods.OliveOil_1_Tbsp,
        Foods.WheatBerries_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Tofu { get; } = new(
        "tofu",
        Foods.Tofu_1_5_Block,
        Foods.PumpkinSeeds_30_Grams,
        Foods.Farro_52_Gram,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarleyAndRice { get; } = new(
        "pearled barley and rice",
        Foods.PearledBarley_45_Grams,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping WorkoutShake { get; } = new(
        "workout shake",
        Foods.PeaProtein_1_Scoop,
        Foods.FatToCarbConversion,
        Foods.OrangeJuice_1_Cup,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);
}
