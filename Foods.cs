namespace SystemOfEquations;

internal static class Foods
{
    public static Food AlmondButter_1_Tbsp { get; } = new("almond butter", 1, "tbsp",
        new(Cals: 130, new(P: 4.5, F: 11.5, C: 4.5 - 2.5)));
    public static Food AlmondMilk_2_Cup { get; } = new("almond milk", 2, "cup",
        new(Cals: 59, new(P: 2, F: 5.1, C: 2 - 2)));
    public static Food BlueBerries_1_Cup { get; } = new("frozen blueberries", 1, "cup",
        new(Cals: 80, new(P: 0, F: 0, C: 19 - 6)));
    public static Food BrownRice_1_Cup { get; } = new("brown rice", 1, "cup",
        new(Cals: 688, new(P: 14.3, F: 5.1, C: 144.7 - 6.5)));
    public static Food Cassein_1_Scoop { get; } = new("cassein", 1, "scoop",
        new(Cals: 100, new(P: 25, F: 0, C: 1 - 0)));
    public static Food ChiaSeeds_2_5_Tbsp { get; } = new("chia seeds", 2.5, "tbsp",
        new(Cals: 150, new(P: 5, F: 9, C: 13 - 10)));
    public static Food Edamame_1_4_Cup { get; } = new("edamame", 0.25, "cup",
        new(Cals: 140, new(P: 13, F: 6, C: 11 - 4)));
    public static Food Gluten_2_Tbsp { get; } = new("gluten", 2, "tbsp",
        new(Cals: 60, new(P: 11.5, F: 0.5, C: 2 - 0)));
    public static Food NutritionalYeast_1_Tbsp { get; } = new(
        "nutritional yeast",
        1, "tbsp",
        new(Cals: 20, new(P: 2.7, F: 0.2, C: 1.7 - 1)));
    public static Food Oatmeal_1_2_Cup { get; } = new("oatmeal", 0.5, "cup",
        new(Cals: 140, new(P: 5, F: 2.5, C: 27 - 4)));
    public static Food OliveOil_1_Tbsp { get; } = new("olive oil", 1, "tbsp",
        new(Cals: 119, new(P: 0, F: 13.5, C: 0)));
    public static Food PumpkinSeeds_1_Cup { get; } = new Food("pumpkin seeds", 1, "cup",
        new(Cals: 520, new(P: 24, F: 24, C: 64 - 24)));
    public static Food Farro_1_Cup { get; } = new Food("farro", 1, "cup",
        new(Cals: 760, new(P: 24, F: 4, C: 152 - 20)));
    public static Food Seitan_Yeast_1_Tbsp_Gluten_2x { get; } = new("Seitan, nutritional yeast, 2x gluten",
        1, "tbsp nutritional yeast",
        NutritionalYeast_1_Tbsp.NutritionalInformation + Gluten_2_Tbsp.NutritionalInformation);
    public static Food Seitan_Yeast_1_Cup_Gluten_2x { get; } = new(Seitan_Yeast_1_Tbsp_Gluten_2x.Name,
        1, "cup nutritional yeast",
        Seitan_Yeast_1_Tbsp_Gluten_2x.NutritionalInformation * 16);
    public static Food Tofu_1_5_block { get; } = new ("tofu", 0.2, "block", new(Cals: 130, new(P: 14, F: 7, C: 0)));

    public static Food Whey_1_Scoop { get; } = new("whey", 1, "scoop", new(Cals: 130, new(P: 30, F: 0.5, C: 1 - 0)));
}
