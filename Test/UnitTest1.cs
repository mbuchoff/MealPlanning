using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        const int pServings = 1, fServings = 2, cServings = 3;
        var m = new Meal("m", new(pServings, fServings, cServings),
            new FoodGrouping("fg", pFood, fFood, cFood, FoodGrouping.PreparationMethodEnum.PrepareAsNeeded));
        var hs = m.Servings.Select(s => new
        {
            s.Name,
            s.NutritionalInformation,
            Servings = s.NutritionalInformation.ServingUnits,
        }).OrderBy(x => x.Name);
        Assert.Equivalent(new[]
        {
            new
            {
                cFood.Name,
                NutritionalInformation = new NutritionalInformation(
                    cServings, ServingUnits.None, Cals: 4 * cServings, P: 0, F: 0, CTotal: cServings, CFiber: 0),
                Servings = cServings,
            },
            new
            {
                fFood.Name,
                NutritionalInformation = new NutritionalInformation(
                    fServings, ServingUnits.None, Cals: 9 * fServings, P: 0, F: fServings, CTotal: 0, CFiber: 0),
                Servings = fServings,
            },
            new
            {
                pFood.Name,
                NutritionalInformation = new NutritionalInformation(
                    pServings, ServingUnits.None, Cals: 4 * pServings, P: pServings, F: 0, CTotal: 0, CFiber: 0),
                Servings = pServings,
            },
        }, hs);
    }

    [Fact]
    public void Meal_Should_Try_FoodGroupings_In_Sequence_Until_One_Succeeds()
    {
        // Create a FoodGrouping that will fail (impossible to achieve target macros)
        var impossibleFoodGrouping = new FoodGrouping(
            "impossible",
            pFood,  // Only provides protein
            pFood,  // Only provides protein (no fat)
            pFood,  // Only provides protein (no carbs)
            FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

        // Create a FoodGrouping that will succeed
        var workingFoodGrouping = new FoodGrouping(
            "working",
            pFood,
            fFood,
            cFood,
            FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

        // Target macros that should work with second FoodGrouping but not first
        var targetMacros = new Macros(P: 1, F: 2, C: 3);

        // Create meal with array of FoodGroupings
        var meal = Meal.WithFallbacks("test meal", targetMacros, impossibleFoodGrouping, workingFoodGrouping);

        // Verify that servings were calculated correctly (this triggers the calculation)
        var servings = meal.Servings.ToList();
        Assert.Equal(3, servings.Count);

        // Verify that the meal used the working FoodGrouping (second one)
        Assert.Equal(workingFoodGrouping, meal.ActualFoodGrouping);
    }

    [Fact]
    public void Meal_Should_Throw_When_All_FoodGroupings_Fail()
    {
        // Create two FoodGroupings that will both fail
        var impossibleFoodGrouping1 = new FoodGrouping(
            "impossible1",
            pFood,  // Only provides protein
            pFood,  // Only provides protein (no fat)
            pFood,  // Only provides protein (no carbs)
            FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

        var impossibleFoodGrouping2 = new FoodGrouping(
            "impossible2",
            fFood,  // Only provides fat (no protein)
            fFood,  // Only provides fat
            fFood,  // Only provides fat (no carbs)
            FoodGrouping.PreparationMethodEnum.PrepareAsNeeded);

        // Target macros that require all three macronutrients
        var targetMacros = new Macros(P: 1, F: 2, C: 3);

        // Create meal with array of impossible FoodGroupings
        var meal = Meal.WithFallbacks("test meal", targetMacros, impossibleFoodGrouping1, impossibleFoodGrouping2);

        // Should throw when trying to calculate servings since all FoodGroupings fail
        Assert.Throws<Exception>(() => meal.Servings.ToList());
    }

    [Fact]
    public void Meal_Should_Throw_When_No_FoodGroupings_Provided()
    {
        var targetMacros = new Macros(P: 1, F: 2, C: 3);

        // Should throw when creating meal with empty array
        Assert.Throws<ArgumentException>(() => Meal.WithFallbacks("test meal", targetMacros));
    }

    private static readonly FoodServing pFood = new("1g p",
            new(ServingUnits: 1, ServingUnits.None, Cals: 4, P: 1, F: 0, CTotal: 0, CFiber: 0));
    private static readonly FoodServing fFood = new("1g f",
        new(ServingUnits: 1, ServingUnits.None, Cals: 9, P: 0, F: 1, CTotal: 0, CFiber: 0));
    private static readonly FoodServing cFood = new("1g c",
        new(ServingUnits: 1, ServingUnits.None, Cals: 4, P: 0, F: 0, CTotal: 1, CFiber: 0));
}
