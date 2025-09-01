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
            new("fg", pFood, fFood, cFood, FoodGrouping.PreparationMethodEnum.PrepareAsNeeded));
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

    private static readonly FoodServing pFood = new("1g p",
            new(ServingUnits: 1, ServingUnits.None, Cals: 4, P: 1, F: 0, CTotal: 0, CFiber: 0));
    private static readonly FoodServing fFood = new("1g f",
        new(ServingUnits: 1, ServingUnits.None, Cals: 9, P: 0, F: 1, CTotal: 0, CFiber: 0));
    private static readonly FoodServing cFood = new("1g c",
        new(ServingUnits: 1, ServingUnits.None, Cals: 4, P: 0, F: 0, CTotal: 1, CFiber: 0));
}
