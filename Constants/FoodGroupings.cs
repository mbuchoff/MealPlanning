namespace SystemOfEquations.Constants;

internal static class FoodGroupings
{
    public static FoodGrouping OatmealAndEdamame { get; } =
        new("oatmeal and edamame", Foods.Edamame_1_Scoop, Foods.AlmondButter_1_Tbsp, Foods.Oatmeal_Sprouts_1_Scoop);
    public static FoodGrouping BlueberryOatmealAndEdamame { get; } =
        new("blueberry oatmeal and edamame", [new(Foods.Blueberries_1_Scoop, Servings: 3)],
        OatmealAndEdamame.PFood, OatmealAndEdamame.FFood, OatmealAndEdamame.CFood);

    public static FoodGrouping OatmealWithProteinPowder { get; } = new(
        "oatmeal with protein powder",
        Foods.Oatmeal_Sprouts_1_Scoop, Foods.ChiaSeeds_2_5_Tbsp, Foods.PeaProtein_1_Tbsp);

    public static FoodGrouping BlueBerryOatmealWithProteinPowder { get; } =
        new("blueberry oatmeal with protein powder", [new(Foods.Blueberries_1_Scoop, Servings: 3)],
        OatmealWithProteinPowder.PFood, OatmealWithProteinPowder.FFood, OatmealWithProteinPowder.CFood);

    public static FoodGrouping ProteinShake { get; } =
        new("protein shake", [new(Foods.AlmondMilk_2_Cup, Servings: 1)],
        Foods.PeaProtein_1_Scoop, Foods.ChiaSeeds_2_5_Tbsp, Foods.Blueberries_1_Scoop);

    public static FoodGrouping Seitan { get; } =
        new("seitan", Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x, Foods.OliveOil_1_Tbsp, Foods.WheatBerries_45_Grams);

    public static FoodGrouping Tofu { get; } =
        new("tofu", Foods.Tofu_1_5_Block, Foods.PumpkinSeeds_30_Grams, Foods.Farro_52_Gram);

    public static FoodGrouping RiceAndEdamame { get; } =
        new("rice and edamame", Foods.Edamame_35_Grams, Foods.PumpkinSeeds_30_Grams, Foods.BrownRice_45_Grams);

    public static FoodGrouping WheatBerriesRice { get; } = new(
        "wheat berries and rice",
        [new(Foods.Farro_52_Gram, Servings: 0.648)],
        Foods.WheatBerries_45_Grams, Foods.OliveOil_1_Tbsp, Foods.BrownRice_45_Grams);
}
