namespace SystemOfEquations;

internal record Food(string Name, NutritionalInformation NutritionalInformation)
{
    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.Servings)} {Name}";

    public Food Convert(double servings, ServingUnit servingUnit, double multiplier) => new(
        this.Name, this.NutritionalInformation.Convert(servings, servingUnit, multiplier));
}
