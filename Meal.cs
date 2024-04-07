using System.Text;
using SystemOfEquations.Extensions;

namespace SystemOfEquations;

internal class Meal
{
    public Meal(string name, Macros macros, FoodGrouping foodGrouping)
    {
        var pMacros = foodGrouping.PFood.NutritionalInformation.Macros;
        var fMacros = foodGrouping.FFood.NutritionalInformation.Macros;
        var cMacros = foodGrouping.CFood.NutritionalInformation.Macros;

        var remainingMacros = macros - foodGrouping.StaticHelpings.Sum(h => h.Macros);
        (var pFoodServings, var fFoodServings, var cFoodServings) = Equation.Solve(
            new(pMacros.P, fMacros.P, cMacros.P, remainingMacros.P),
            new(pMacros.F, fMacros.F, cMacros.F, remainingMacros.F),
            new(pMacros.C, fMacros.C, cMacros.C, remainingMacros.C));
        Name = name;
        Macros = macros;
        Helpings = foodGrouping.StaticHelpings.Append(
        [
            new Helping(foodGrouping.PFood, pFoodServings),
            new Helping(foodGrouping.FFood, fFoodServings),
            new Helping(foodGrouping.CFood, cFoodServings),
        ]);
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
