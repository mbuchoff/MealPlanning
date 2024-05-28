namespace SystemOfEquations;

internal record NutritionalInformation(
    double Servings, ServingUnit ServingUnit,
    double Cals,
    double P,
    double F,
    double CTotal, double CFiber)
{
    public NutritionalInformation Combine(NutritionalInformation that, double ratio)
    {
        if (this.ServingUnit != that.ServingUnit)
        {
            throw new Exception($"{nameof(ServingUnit)}s are different");
        }

        var thisMultiplier = 1.0 / Servings;
        var thatMultiplier = ratio / that.Servings;

        return new(
            1, ServingUnit,
            (Cals * thisMultiplier) + (that.Cals * thatMultiplier),
            (P * thisMultiplier) + (that.P * thatMultiplier),
            (F * thisMultiplier) + (that.F * thatMultiplier),
            (CTotal * thisMultiplier) + (that.CTotal * thatMultiplier),
            (CFiber * thisMultiplier) + (that.CFiber * thatMultiplier));
    }

    public Macros Macros => new(P, F, CTotal - CFiber);

    public static NutritionalInformation operator *(NutritionalInformation n, double d) => new(
        n.Servings * d, n.ServingUnit,
        n.Cals * d,
        n.P * d,
        n.F * d,
        n.CTotal * d,
        n.CFiber * d);

    public string ToNutrientsString() => $"{Cals:F0} cals, {Macros}, {CFiber:F0}g fiber";
}

internal static class NutritionalInformationExtensions
{
    public static NutritionalInformation Sum(
        this IEnumerable<NutritionalInformation> nutritionalInformations,
        double newServings, ServingUnit newServingUnit) => new(
            newServings, newServingUnit,
            nutritionalInformations.Sum(n => n.Cals),
            nutritionalInformations.Sum(n => n.P),
            nutritionalInformations.Sum(n => n.F),
            nutritionalInformations.Sum(n => n.CTotal),
            nutritionalInformations.Sum(n => n.CFiber));
}