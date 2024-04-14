namespace SystemOfEquations;

public record Macros(double P, double F, double C)
{
    public static Macros operator -(Macros m) => new(-m.P, -m.F, -m.C);
    public static Macros operator -(Macros m1, Macros m2) => m1 + (-m2);
    public static Macros operator +(Macros m1, Macros m2) => new(m1.P + m2.P, m1.F + m2.F, m1.C + m2.C);
    public static Macros operator *(Macros m, double d) => new(m.P * d, m.F * d, m.C * d);

    public Macros CloneWithTweakedMacros(double pMultiplier, double fMultiplier, double cMultiplier) =>
        new(P * pMultiplier, F * fMultiplier, C * cMultiplier);
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
