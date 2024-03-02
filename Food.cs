namespace SystemOfEquations;

internal record Food(
    string Name,
    double Servings,
    ServingUnit ServingUnit,
    NutritionalInformation NutritionalInformation)
{
    public string ToString(double quantity) => $"{ServingUnit.ToString(quantity * Servings)} {Name}";
}
