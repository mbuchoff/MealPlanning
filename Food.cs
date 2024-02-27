namespace SystemOfEquations;

internal record Food(string Name, double Servings, string ServingsName, NutritionalInformation NutritionalInformation)
{
    public string ToString(double quantity) => $"{quantity * Servings} {ServingsName} {Name}";
}
