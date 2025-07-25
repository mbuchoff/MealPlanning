﻿using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data;

internal static class FoodGroupings
{
    public static FoodGrouping BlueberriesOatmealAndEdamame { get; } = new(
        "blueberries, oatmeal, and edamame",
        [new(Foods.BlueBerries_1_Scoop, Servings: 2)],
        Foods.Edamame_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Ezekial(bool withEdamame = true) => new(
        "Ezekial",
        [new(Foods.AlmondMilk_1_Scoop, Servings: 2)],
        Foods.PumpkinSeeds_1_Scoop,
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToFatConversion,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping OatmealWithAlmondButter { get; } = new(
        "oatmeal with almond butter",
        Foods.Oats_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        Foods.WheatBran_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping BlueBerryOatmeal { get; } = new(
        "blueberry oatmeal",
        [new(Foods.BlueBerries_1_Scoop, Servings: 3)],
        Foods.WheatBran_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Oatmeal(bool withEdamame = true) => new(
        "Oatmeal and edamame",
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
        Foods.AlmondButter_1_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping EnglishMuffinsAndPasta(int englishMuffins, bool withEdamame = true) => new(
        "English muffins and pasta",
        englishMuffins == 0 ? [] : [new(Foods.Ezekiel_English_Muffin, Servings: englishMuffins)],
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
        Foods.OliveOil_1_Tbsp,
        Foods.Whole_Grain_Pasta_56_Grams,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping ProteinShake { get; } = new(
        "protein shake",
        [new(Foods.AlmondMilk_2_Cup, Servings: 1)],
        Foods.PeaProtein_1_Scoop,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.BlueBerries_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Rice_BlackBeans_OliveOil { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Rice_BlackBeans_PumpkinSeeds { get; } = new(
        "rice and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.PumpkinSeeds_30_Grams,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarley_BlackBeans_OliveOil { get; } = new(
        "pearled barley and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.OliveOil_1_Tbsp,
        Foods.PearledBarley_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarley_BlackBeans_PumpkinSeeds { get; } = new(
        "pearled barley and beans",
        Foods.BlackBeans_Sprouts_45g,
        Foods.PumpkinSeeds_30_Grams,
        Foods.PearledBarley_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping Seitan { get; } = new(
        "seitan",
        Foods.Seitan_Sprouts_Yeast_1_Gram_Gluten_4x,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping ToastedWheatfuls { get; } = new(
        "toasted wheatfuls",
        [new(Foods.AlmondMilk_2_Cup, Servings: 0.5M)],
        Foods.ToastedWheatfuls,
        Foods.PumpkinSeeds_30_Grams,
        Foods.Edamame_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded);

    public static FoodGrouping Tofu { get; } = new(
        "tofu",
        Foods.Tofu_91_Grams,
        Foods.PumpkinSeeds_30_Grams,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping PearledBarleyAndRice { get; } = new(
        "pearled barley and rice",
        Foods.PearledBarley_45_Grams,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping TofuAndWheatBerries { get; } = new(
        "wheat berries",
        Foods.Tofu_91_Grams,
        Foods.PumpkinSeeds_30_Grams,
        Foods.WheatBerries_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    public static FoodGrouping WorkoutShake { get; } = new(
        "workout shake",
        Foods.PeaProtein_1_Scoop,
        Foods.FatToCarbConversion,
        Foods.OrangeJuice_1_Cup,
        PreparationMethodEnum.PrepareAsNeeded);
}
