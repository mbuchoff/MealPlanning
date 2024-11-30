using SystemOfEquations.Extensions;

namespace SystemOfEquations;

internal record NutritionalInformation(
    IEnumerable<(double Servings, ServingUnit ServingUnit)> ServingInfo,
    double Cals,
    double P,
    double F,
    double CTotal, double CFiber)
{
    public NutritionalInformation Combine(NutritionalInformation that, ServingUnit servingUnit, double ratio)
    {
        (double Servings, ServingUnit ServingUnit)? maybeThisServingInfo = ServingInfo
            .FirstOrDefault(si => si.ServingUnit == servingUnit);
        (double Servings, ServingUnit ServingUnit)? maybeThatServingInfo = that.ServingInfo
            .FirstOrDefault(si => si.ServingUnit == servingUnit);

        if (maybeThisServingInfo == null || maybeThisServingInfo == null)
        {
            throw new Exception($"Cannot find {servingUnit} in both {nameof(NutritionalInformation)}s");
        }

        var thisServingInfo = maybeThatServingInfo.Value;
        var thatServingInfo = maybeThatServingInfo.Value;

        var thisMultiplier = 1.0 / thisServingInfo.Servings;
        var thatMultiplier = ratio / thatServingInfo.Servings;

        return new(
            [(1, thisServingInfo.ServingUnit)],
            (Cals * thisMultiplier) + (that.Cals * thatMultiplier),
            (P * thisMultiplier) + (that.P * thatMultiplier),
            (F * thisMultiplier) + (that.F * thatMultiplier),
            (CTotal * thisMultiplier) + (that.CTotal * thatMultiplier),
            (CFiber * thisMultiplier) + (that.CFiber * thatMultiplier));
    }

    public Macros Macros => new(P, F, CTotal - CFiber);

    public static NutritionalInformation operator *(NutritionalInformation n, double d) => new(
        n.ServingInfo.Select(si => (si.Servings * d, si.ServingUnit)).ToList(),
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
            [(newServings, newServingUnit)],
            nutritionalInformations.Sum(n => n.Cals),
            nutritionalInformations.Sum(n => n.P),
            nutritionalInformations.Sum(n => n.F),
            nutritionalInformations.Sum(n => n.CTotal),
            nutritionalInformations.Sum(n => n.CFiber));
}