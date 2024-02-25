namespace SystemOfEquations;

internal static class FoodGroupings
{
    public static FoodGrouping Oatmeal { get; } =
        new(Foods.Edamame_1_4_Cup, Foods.AlmondButter_1_Tbsp, Foods.Oatmeal_1_2_Cup);
    public static FoodGrouping ProteinShake { get; } =
        new(Foods.Whey_1_Scoop, Foods.AlmondButter_1_Tbsp, Foods.BlueBerries_1_Cup);
    public static FoodGrouping Seitan { get; } =
        new(Foods.Seitan_Yeast_1_Tbsp_Gluten_2x, Foods.OliveOil_1_Tbsp, Foods.BrownRice_1_Cup);
    public static FoodGrouping Tofu { get; } =
        new(Foods.Tofu_1_5_block, Foods.PumpkinSeeds_1_Cup, Foods.Farro_1_Cup);
}
