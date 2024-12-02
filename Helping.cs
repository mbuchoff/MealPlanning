namespace SystemOfEquations;

internal record Helping(Food Food, ServingUnit ServingUnit, double Servings)
{
    public override string ToString() => Food.ToString(ServingUnit, Servings);

    public Macros Macros => Food.NutritionalInformation.Macros * Servings;
    public NutritionalInformation NutritionalInformation => Food.NutritionalInformation * Servings;
    public static Helping operator *(Helping h, double d) => new(h.Food, h.ServingUnit, h.Servings * d);
}

internal static class HelpingExtensions
{
    public static IEnumerable<Helping> CombineLikeHelpings(this IEnumerable<Helping> helpings) => helpings
        .GroupBy(h => (h.Food, h.ServingUnit))
        .Select(foodGrouping => new Helping(
            foodGrouping.Key.Food,
            foodGrouping.Key.ServingUnit,
            Servings: foodGrouping.Select(h => h.Servings).Sum()));
}