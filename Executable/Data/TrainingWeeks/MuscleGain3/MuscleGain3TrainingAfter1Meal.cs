using System.Runtime.ConstrainedExecution;
using static SystemOfEquations.FoodGrouping;

namespace SystemOfEquations.Data.TrainingWeeks.MuscleGain3;

internal record MuscleGain3TrainingAfter1Meal : TrainingWeekBase
{
    internal MuscleGain3TrainingAfter1Meal(decimal targetGramsProteinPerDay) : base(
        "Muscle Gain 3",
        nonworkoutMeals:
        [
            new Meal("Waking",
                new(P: MuscleGainProteinPerMealOnNonworkoutDay(targetGramsProteinPerDay), F: 10, C: 60),
                Cereal),
            new("3-5 hours after last meal",
                new(P: MuscleGainProteinPerMealOnNonworkoutDay(targetGramsProteinPerDay), F: 20, C: 60),
                Cereal),
            new("3-5 hours after last meal",
                new(P: MuscleGainProteinPerMealOnNonworkoutDay(targetGramsProteinPerDay), F: 20, C: 60),
                //FoodGroupings.EnglishMuffinsAndPasta(0)),
                SeitanAndEnglishMuffin),
            new Meal("3-5 hours after last meal",
                new(P: MuscleGainProteinPerMealOnNonworkoutDay(targetGramsProteinPerDay), F: 20, C: 60),
                //SeitanAndEnglishMuffin),
                FoodGroupings.EnglishMuffinsAndPasta(0)),
            new Meal("Bedtime",
                new Macros(P: MuscleGainProteinPerMealOnNonworkoutDay(targetGramsProteinPerDay), F: 25, C: 0),
                NonworkoutBedtimeFoodGroupings),
        ],
        runningMeals:
        [
            new Meal("1-3 hours before workout",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 80),
                Oatmeal(blueberryScoops: 3)),
            new("1/2 shake during workout, 1/2 right after",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 0, C: 55),
                WorkoutMeal),
            new Meal("40 minutes after workout",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 10, C: 120),
                //SeitanAndEnglishMuffin),
                //Cereal),
                FoodGroupings.EnglishMuffinsAndPasta(0)),
            new("2-4 hours after last meal",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 100),
                FoodGroupings.EnglishMuffinsAndPasta(0)),
                //EnglishMuffin),
            new("3-5 hours after last meal",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 50),
                ToastAndAlmondButter(withOrangeJuice: false)),
            new Meal("Bedtime",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 25, C: 35),
                //FoodGroupings.EnglishMuffinsAndPasta(englishMuffins: 0)),
                Cereal),
        ],
        xfitMeals:
        [
            new Meal("1-3 hours before workout",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 80),
                Oatmeal(blueberryScoops: 3)),
            new("1/2 shake during workout, 1/2 right after",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 0, C: 55),
                WorkoutMeal),
            new Meal("40 minutes after workout",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 10, C: 120),
                ToastAndAlmondButter(withOrangeJuice: true)),
            new("2-4 hours after last meal",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 100),
                //EnglishMuffinsAndRice),
                FoodGroupings.EnglishMuffinsAndPasta(0)),
            new("3-5 hours after last meal",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 20, C: 100),
                //EnglishMuffinsAndRice),
                FoodGroupings.EnglishMuffinsAndPasta(0)),
            new Meal("Bedtime",
                new(P: MuscleGainProteinPerMealOnWorkoutDay(targetGramsProteinPerDay), F: 25, C: 65),
                Cereal),
        ])
    {

    }

    private static readonly FallbackChain Cereal = new(
        new FoodGrouping(
            "Fiber One",
            [Foods.AlmondMilk_1_Scoop * 2],
            Foods.PumpkinSeeds_1_Scoop,
            Foods.Edamame_1_Scoop,
            Foods.FiberOne_2_3_Cup,
            PreparationMethodEnum.PrepareAsNeeded),
        new FoodGrouping(
            "Fiber One",
            [Foods.AlmondMilk_1_Scoop * 2],
            Foods.Edamame_1_Scoop,
            Foods.FatToCarbConversion,
            Foods.FiberOne_2_3_Cup,
            PreparationMethodEnum.PrepareAsNeeded));

    private static FallbackChain ToastAndAlmondButter(bool withOrangeJuice)
    {
        FoodServing[] staticServings = withOrangeJuice ? [Foods.OrangeJuice_1_Cup * 2] : [];
        return new(
            new FoodGrouping(
                "toast and almond butter",
                staticServings,
                Foods.Edamame_1_Scoop,
                Foods.AlmondButter_1_Tbsp,
                Foods.Ezekial_Bread_Low_Sodium_1_Slice,
                PreparationMethodEnum.PrepareAsNeeded),
            new FoodGrouping(
                "toast and almond butter",
                staticServings,
                Foods.Edamame_1_Scoop,
                Foods.FatToCarbConversion,
                Foods.Ezekial_Bread_Low_Sodium_1_Slice,
                PreparationMethodEnum.PrepareAsNeeded));
    }

    private static FallbackChain WorkoutMeal { get; } = new(
        new FoodGrouping(
            "workout shake",
            Foods.Edamame_1_Scoop,
            Foods.FatToCarbConversion,
            Foods.Ezekial_Bread_Low_Sodium_1_Slice,
            PreparationMethodEnum.PrepareAsNeeded),
        new FoodGrouping(
            "workout shake",
            Foods.FatToCarbConversion,
            Foods.FatToCarbConversion,
            Foods.OrangeJuice_1_Cup,
            PreparationMethodEnum.PrepareAsNeeded),
        new FoodGrouping(
            "workout shake",
            Foods.ProteinToCarbConversion,
            Foods.FatToCarbConversion,
            Foods.OrangeJuice_1_Cup,
            PreparationMethodEnum.PrepareAsNeeded));

    private static FallbackChain Oatmeal(int blueberryScoops) => new(
        new[] { Foods.Edamame_1_Scoop, Foods.ProteinToFatConversion }
            .Select(pFood => new FoodGrouping(
                "blueberries and oatmeal",
                [
                    Foods.Ezekiel_English_Muffin, Foods.AlmondButter_1_Tbsp,
                    Foods.BlueBerries_1_Scoop * blueberryScoops,
                    Foods.Creatine_1_Scoop
                ],
                pFood,
                Foods.ChiaSeeds_2_5_Tbsp,
                Foods.Oats_1_Scoop,
                PreparationMethodEnum.PrepareAsNeeded)).ToArray());

    private static FallbackChain EnglishMuffinsAndRice { get; } = new(
        new FoodGrouping(
            "rice",
            [Foods.Ezekiel_English_Muffin, Foods.AlmondButter_1_Tbsp],
            Foods.WheatBerries_45_Grams,
            Foods.PumpkinSeeds_30_Grams,
            Foods.BrownRice_45_Grams,
            PreparationMethodEnum.PrepareInAdvance),
        new FoodGrouping(
            "rice",
            [Foods.Ezekiel_English_Muffin, Foods.AlmondButter_1_Tbsp],
            Foods.WheatBerries_45_Grams,
            Foods.FatToCarbConversion,
            Foods.BrownRice_45_Grams,
            PreparationMethodEnum.PrepareInAdvance),
        new FoodGrouping(
            "rice",
            [Foods.Ezekiel_English_Muffin, Foods.AlmondButter_1_Tbsp],
            Foods.WheatBerries_45_Grams,
            Foods.FatToCarbConversion,
            Foods.ProteinToCarbConversion,
            PreparationMethodEnum.PrepareInAdvance));

    private static readonly FoodGrouping SeitanAndEnglishMuffin = new(
        "seitan and english muffin",
        [Foods.Ezekiel_English_Muffin],
        Foods.Seitan_Sprouts_Yeast_1_Gram_Gluten_4x,
        Foods.OliveOil_1_Tbsp,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    private static readonly FoodGrouping TofuAndEnglishMuffin = new(
        "tofu and english muffin",
        [Foods.Ezekiel_English_Muffin],
        Foods.Tofu_91_Grams,
        Foods.PumpkinSeeds_30_Grams,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance);

    private static FallbackChain RestDayCookingFoodGrouping { get; } = new(
        new FoodGrouping[]
        {
            new FoodGrouping("rice",
                [],
                Foods.WheatBerries_45_Grams,
                Foods.PumpkinSeeds_30_Grams,
                Foods.BrownRice_45_Grams,
                PreparationMethodEnum.PrepareInAdvance),
            TofuAndEnglishMuffin
        });

    private static readonly FallbackChain NonworkoutWakingOatmealFoodGroupings = new(
        [.. new[] { Foods.AlmondButter_1_Tbsp, Foods.FatToCarbConversion }.Select(fFood =>
        new FoodGrouping("Blueberry oatmeal shake",
            [
                Foods.BlueBerries_1_Scoop * 2,
                Foods.Creatine_1_Scoop,
            ],
            Foods.Edamame_1_Scoop,
            fFood,
            Foods.Oats_1_Scoop,
            PreparationMethodEnum.PrepareAsNeeded))]);

    private static readonly FallbackChain NonworkoutBedtimeFoodGroupings = new(
        [.. new[] { Foods.Whole_Grain_Pasta_56_Grams, Foods.FatToCarbConversion }.SelectMany(cFood =>
            new[] { Foods.PumpkinSeeds_1_Scoop, Foods.FatToProteinConversion }.Select(fFood =>
        new FoodGrouping("Edamame",
            Foods.Edamame_1_Scoop,
            fFood,
            cFood,
            PreparationMethodEnum.PrepareAsNeeded)))]);

}
