namespace SystemOfEquations.Constants;

internal static class FoodGroupings
{
    public static FoodGrouping OatmealAndEdamame { get; } = new(
        "oatmeal and edamame",
        Foods.Edamame_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        Foods.Oatmeal_Sprouts_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueberryOatmealAndEdamame { get; } = new(
        "blueberry oatmeal and edamame",
        [new(Foods.BlueBerries_1_Scoop, Servings: 3)],
        OatmealAndEdamame.PFood, OatmealAndEdamame.FFood, OatmealAndEdamame.CFood,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping OatmealWithProteinPowder { get; } = new(
        "oatmeal with protein powder",
        Foods.Oatmeal_Sprouts_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.PeaProtein_1_Tbsp,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueBerryOatmealWithProteinPowder { get; } = new(
        "blueberry oatmeal with protein powder",
        [new(Foods.BlueBerries_1_Scoop, Servings: 3)],
        OatmealWithProteinPowder.PFood,
        OatmealWithProteinPowder.FFood,
        OatmealWithProteinPowder.CFood,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping ProteinShake { get; } = new(
        "protein shake",
        [new(Foods.AlmondMilk_2_Cup, Servings: 1)],
        Foods.PeaProtein_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.BlueBerries_1_Scoop,
        FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping RiceAndBeans { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping WheatBerriesAndEdamame { get; } = new(
        "wheat berries and edamame",
        Foods.Edamame_35_Grams,
        Foods.PumpkinSeeds_30_Grams,
        Foods.WheatBerries_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Seitan { get; } = new(
        "seitan",
        Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x,
        Foods.OliveOil_1_Tbsp,
        Foods.WheatBerries_45_Grams,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Tofu { get; } = new(
        "tofu",
        Foods.Tofu_1_5_Block,
        Foods.PumpkinSeeds_30_Grams,
        Foods.Farro_52_Gram,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping WheatBerriesRice { get; } = new(
        "wheat berries and rice",
        Foods.WheatBerries_45_Grams,
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
