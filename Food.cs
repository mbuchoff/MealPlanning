namespace SystemOfEquations;

internal record Food(
    string Name,
    double Servings,
    ServingUnit ServingUnit,
    NutritionalInformation NutritionalInformation)
{
    public string ToString(double quantity) =>
        $"{(quantity * Servings).ToString($"{ServingUnit.ToString(Servings)} {Name}")}";
}
