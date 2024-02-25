namespace SystemOfEquations;

public record Food(string Name, double Servings, string ServingsName, double Cals, Macros Macros)
{
    public string ToString(double quantity) => $"{quantity * Servings} {ServingsName} {Name}";
}
