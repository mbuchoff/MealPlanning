namespace SystemOfEquations;

public static class Foods
{
    public static Food AlmondButter_1_Tbsp { get; } = new("1 tbsp almond butter", Cals: 130,
        new(P: 4.5, F: 11.5, C: 4.5 - 2.5));
    public static Food AlmondMilk_2_Cup { get; } = new("2 cup almond milk", Cals: 59, new(P: 2, F: 5.1, C: 2 - 2));
    public static Food BlueBerries_1_Cup { get; } = new("1 cup frozen blueberries", Cals: 80,
        new(P: 0, F: 0, C: 19 - 6));
    public static Food BrownRice_1_Cup { get; } = new("1 cup brown rice", Cals: 688,
        new(P: 14.3, F: 5.1, C: 144.7 - 6.5));
    public static Food Cassein_1_Scoop { get; } = new("1 scoop cassein", Cals: 100, new(P: 25, F: 0, C: 1 - 0));
    public static Food Edamame_1_4_Cup { get; } = new("1/4 cup edamame", Cals: 140, new(P: 13, F: 6, C: 11 - 4));
    public static Food Gluten_2_Tbsp { get; } = new("2 tbsp gluten", Cals: 60, new(P: 11.5, F: 0.5, C: 2 - 0));
    public static Food NutritionalYeast_1_Tbsp { get; } = new("1 tbsp nutritional yeast",
        Cals: 20, new(P: 2.7, F: 0.2, C: 1.7 - 1));
    public static Food Oatmeal_1_2_Cup { get; } = new("1/2 cup oatmeal", Cals: 140, new(P: 5, F: 2.5, C: 27 - 4));
    public static Food OliveOil_1_Tbsp { get; } = new("1 tbsp olive oil", Cals: 119, new(P: 0, F: 13.5, C: 0));
    public static Food PumpkinSeeds_1_Cup { get; } = new Food("1 cup pumpkin seeds", Cals: 520,
        new(P: 24, F: 24, C: 64 - 24));
    public static Food Farro_1_Cup { get; } = new Food("1 cup farro", Cals: 760, new(P: 24, F: 4, C: 152 - 20));
    public static Food Seitan_Yeast_1_Tbsp_Gluten_2x { get; } = new("Seitan, 1 tbsp nutritional yeast, 2x gluten",
        Cals: Foods.NutritionalYeast_1_Tbsp.Cals + Foods.Gluten_2_Tbsp.Cals,
        Foods.NutritionalYeast_1_Tbsp.Macros + Foods.Gluten_2_Tbsp.Macros);
    public static Food Tofu_1_5_block { get; } = new ("1/5 block tofu", Cals: 130, new(P: 14, F: 7, C: 0));

    public static Food Whey_1_Scoop { get; } = new("1 scoop whey", Cals: 130, new(P: 30, F: 0.5, C: 1 - 0));
}
