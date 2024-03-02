namespace SystemOfEquations;

internal record Food(string Name, NutritionalInformation NutritionalInformation)
{
    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.Servings)} {Name}";
}
