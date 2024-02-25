using System.Text;

namespace SystemOfEquations;

public class Meal
{
    public Meal(string name, Macros macros, Food pFood, Food fFood, Food cFood)
    {
        (var pFoodServings, var fFoodServings, var cFoodServings) = Equation.Solve(
            new(pFood.Macros.P, fFood.Macros.P, cFood.Macros.P, macros.P),
            new(pFood.Macros.F, fFood.Macros.F, cFood.Macros.F, macros.F),
            new(pFood.Macros.C, fFood.Macros.C, cFood.Macros.C, macros.C));
        Name = name;
        Macros = macros;
        Helpings = [new(pFood, pFoodServings), new(fFood, fFoodServings), new(cFood, cFoodServings)];
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
