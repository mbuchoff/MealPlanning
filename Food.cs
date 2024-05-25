namespace SystemOfEquations;

internal record Food(
    string Name,
    NutritionalInformation NutritionalInformation,
    Food.AmountWater? Water = null,
    bool IsConversion = false)
{
    public record AmountWater(double Base, double PerServing);

    public string ToString(double quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.Servings)} {Name}" +
        $"{(Water == null ? "" : $", {(Water.Base + Water.PerServing * quantity):f1} cups water")}";

    public Food Convert(
        double servings,
        ServingUnit servingUnit,
        double multiplier,
        AmountWater? water = null) => new(
            Name,
            NutritionalInformation.Convert(servings, servingUnit, multiplier),
            water,
            IsConversion);
}
