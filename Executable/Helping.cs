namespace SystemOfEquations;

public record Helping(Food Food, decimal Servings)
{
    public override string ToString() => Food.ToString(Servings);

    public Macros Macros => Food.NutritionalInformation.Macros * Servings;
    public NutritionalInformation NutritionalInformation => Food.NutritionalInformation * Servings;
    public static Helping operator *(Helping h, decimal d) => new(h.Food, h.Servings * d);
}

internal static class HelpingExtensions
{
    public static IEnumerable<Helping> CombineLikeHelpings(this IEnumerable<Helping> helpings) => helpings
        .GroupBy(h => h.Food)
        .Select(foodGrouping => new Helping(
            Food: foodGrouping.Key,
            Servings: foodGrouping.Select(h => h.Servings).Sum()));
}