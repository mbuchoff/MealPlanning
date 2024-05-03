namespace SystemOfEquations;

internal record Food(
    string Name,
    NutritionalInformation NutritionalInformation,
    double? CupsWaterPerServing = null,
    bool Hidden = false)
{
    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.Servings)} {Name}" +
        $"{(CupsWaterPerServing == null ? "" : $", {CupsWaterPerServing.Value * quantity:f1} cups water")}";

    public Food Convert(
        double servings,
        ServingUnit servingUnit,
        double multiplier,
        double? cupsWaterPerServing = null) => new(
            Name,
            NutritionalInformation.Convert(servings, servingUnit, multiplier),
            cupsWaterPerServing,
            Hidden);
}
