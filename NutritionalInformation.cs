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

        var thisMultiplier = 1.0 / this.Servings;
        var thatMultiplier = ratio / that.Servings;

        return new(
            Servings: 1,
            this.ServingUnit,
            (this.Cals * thisMultiplier) + (that.Cals * thatMultiplier),
            (this.P * thisMultiplier) + (that.P * thatMultiplier),
            (this.F * thisMultiplier) + (that.F * thatMultiplier),
            (this.CTotal * thisMultiplier) + (that.CTotal * thatMultiplier),
            (this.CFiber * thisMultiplier) + (that.CFiber * thatMultiplier));
    }

    public Macros Macros => new(P, F, CTotal - CFiber);

    public NutritionalInformation Convert(double servings, ServingUnit servingUnit, double multiplier) => new(
        servings, servingUnit,
        this.Cals * multiplier,
        this.P * multiplier,
        this.F * multiplier,
        this.CTotal * multiplier,
        this.CFiber * multiplier);
}
