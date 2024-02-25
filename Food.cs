namespace SystemOfEquations;

public record Food(string Name, double Cals, Macros Macros)
{
    public override string ToString() => Name;
}
