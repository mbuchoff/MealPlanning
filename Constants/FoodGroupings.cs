namespace SystemOfEquations.Constants;

internal static class FoodGroupings
{
    public static FoodGrouping Oatmeal { get; } =
        new(Foods.Edamame_1_Scoop, Foods.AlmondButter_1_Tbsp, Foods.Oatmeal_1_Scoop);
    public static FoodGrouping ProteinShake { get; } =
        new(Foods.Cassein_1_Scoop, Foods.ChiaSeeds_2_5_Tbsp, Foods.BlueBerries_1_Cup);
    public static FoodGrouping Seitan { get; } =
        new(Foods.Seitan_Walmart_Yeast_1_Gram_Gluten_4x, Foods.OliveOil_1_Tbsp, Foods.BrownRice_45_Grams);
    public static FoodGrouping Tofu { get; } =
        new(Foods.Tofu_1_5_Block, Foods.PumpkinSeeds_30_Grams, Foods.Farro_52_Gram);
}
