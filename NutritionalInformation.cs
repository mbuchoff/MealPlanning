namespace SystemOfEquations;

internal record NutritionalInformation(
    decimal ServingUnits, ServingUnit ServingUnit,
    decimal Cals,
    decimal P,
    decimal F,
    decimal CTotal, decimal CFiber)
{
    public NutritionalInformation Combine(NutritionalInformation that, decimal ratio)
    {
        if (this.ServingUnit != that.ServingUnit)
        {
            throw new Exception($"{nameof(ServingUnit)}s are different");
        }

        var thisMultiplier = 1.0M / ServingUnits;
        var thatMultiplier = ratio / that.ServingUnits;

        return new(
            1, ServingUnit,
            (Cals * thisMultiplier) + (that.Cals * thatMultiplier),
            (P * thisMultiplier) + (that.P * thatMultiplier),
            (F * thisMultiplier) + (that.F * thatMultiplier),
            (CTotal * thisMultiplier) + (that.CTotal * thatMultiplier),
            (CFiber * thisMultiplier) + (that.CFiber * thatMultiplier));
    }

    public Macros Macros => new(P, F, CTotal - CFiber);

    public static NutritionalInformation operator *(NutritionalInformation n, decimal d) => new(
        n.ServingUnits * d, n.ServingUnit,
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
        decimal newServings, ServingUnit newServingUnit) => new(
            newServings, newServingUnit,
            nutritionalInformations.Sum(n => n.Cals),
            nutritionalInformations.Sum(n => n.P),
            nutritionalInformations.Sum(n => n.F),
            nutritionalInformations.Sum(n => n.CTotal),
            nutritionalInformations.Sum(n => n.CFiber));
}