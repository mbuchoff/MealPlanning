using Microsoft.VisualBasic;

namespace SystemOfEquations;

public record Helping(Food Food, double Servings)
{
    public override string ToString() => Food.Print(Servings);
}