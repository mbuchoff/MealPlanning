namespace SystemOfEquations;

internal record NutritionalInformation(double Servings, ServingUnit ServingUnit, double Cals, Macros Macros)
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
            (this.Macros * thisMultiplier) + (that.Macros * thatMultiplier));
    }
}
