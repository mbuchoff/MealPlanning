namespace SystemOfEquations;

public record Macros(decimal P, decimal F, decimal C)
{
    public static Macros operator -(Macros m) => new(-m.P, -m.F, -m.C);
    public static Macros operator -(Macros m1, Macros m2) => m1 + (-m2);
    public static Macros operator +(Macros m1, Macros m2) => new(m1.P + m2.P, m1.F + m2.F, m1.C + m2.C);
    public static Macros operator *(Macros m, decimal d) => new(m.P * d, m.F * d, m.C * d);
    public static Macros operator *(decimal d, Macros m) => (m * d);
    public static Macros operator /(Macros m, decimal d) => (m * (1/d));

    public Macros CloneWithTweakedMacros(decimal pMultiplier, decimal fMultiplier, decimal cMultiplier) =>
        new(P * pMultiplier, F * fMultiplier, C * cMultiplier);

    public override string ToString()
    {
        var macroDetails = new[]
        {
            new
            {
                Grams = P,
                Name = "P",
                CalsPerGram = 4,
            },
            new
            {
                Grams = F,
                Name = "F",
                CalsPerGram = 9,
            },
            new
            {
                Grams = C,
                Name = "C",
                CalsPerGram = 4,
            },
        };
        var totalCals = macroDetails.Sum(m => m.CalsPerGram * m.Grams);
        if (totalCals == 0)
        {
            return "";
        }

        return string.Join(", ", macroDetails.Select(m =>
            $"{m.Grams:F0} {m.Name} ({m.CalsPerGram * m.Grams / totalCals * 100:F1}%)"));
    }
}

public static class MacrosExtensions
{
    public static Macros Sum<T>(this IEnumerable<T> items, Func<T, Macros> fMacros)
    {
        Macros total = new(P: 0, F: 0, C: 0);
        foreach (var item in items)
        {
            total += fMacros(item);
        }

        return total;
    }
}
