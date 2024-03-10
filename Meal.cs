using System.Text;

namespace SystemOfEquations;

internal class Meal
{
    public Meal(string name, Macros macros, FoodGrouping foodGrouping)
    {
        var pMacros = foodGrouping.PFood.NutritionalInformation.Macros;
        var fMacros = foodGrouping.FFood.NutritionalInformation.Macros;
        var cMacros = foodGrouping.CFood.NutritionalInformation.Macros;

        (var pFoodServings, var fFoodServings, var cFoodServings) = Equation.Solve(
            new(pMacros.P, fMacros.P, cMacros.P, macros.P),
            new(pMacros.F, fMacros.F, cMacros.F, macros.F),
            new(pMacros.C, fMacros.C, cMacros.C, macros.C));
        Name = name;
        Macros = macros;
        Helpings =
        [
            new(foodGrouping.PFood, pFoodServings),
            new(foodGrouping.FFood, fFoodServings),
            new(foodGrouping.CFood, cFoodServings),
        ];
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Name);

        foreach (var helping in Helpings)
        {
            sb.AppendLine(helping.ToString());
        }

        return sb.ToString();
    }

    public string Name { get; }
    public Macros Macros { get; }
    public IEnumerable<Helping> Helpings { get; }
}
