namespace SystemOfEquations.Constants;

internal static class FoodGroupings
{
    public static FoodGrouping OatmealAndEdamame { get; } =
        new(Foods.Edamame_1_Scoop, Foods.AlmondButter_1_Tbsp, Foods.Oatmeal_Walmart_1_Scoop);
    public static FoodGrouping OatmealWithProteinPowder { get; } =
        new(Foods.Oatmeal_Walmart_1_Scoop, Foods.ChiaSeeds_2_5_Tbsp, Foods.PeaProtein_1_Scoop);
    public static FoodGrouping ProteinShake { get; } =
        new(Foods.PeaProtein_1_Scoop, Foods.ChiaSeeds_2_5_Tbsp, Foods.Blueberries_1_Scoop);
    public static FoodGrouping Seitan { get; } =
        new(Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x, Foods.OliveOil_1_Tbsp, Foods.BrownRice_45_Grams);
    public static FoodGrouping Tofu { get; } =
        new(Foods.Tofu_1_5_Block, Foods.PumpkinSeeds_30_Grams, Foods.Farro_52_Gram);
    public static FoodGrouping FarroAndEdamame { get; } =
        new(Foods.Edamame_35_Grams, Foods.PumpkinSeeds_30_Grams, Foods.Farro_52_Gram);
    public static FoodGrouping WheatBerriesAndEdamame { get; } =
        new(Foods.Edamame_35_Grams, Foods.PumpkinSeeds_30_Grams, Foods.WheatBerries_45_Grams);
}
