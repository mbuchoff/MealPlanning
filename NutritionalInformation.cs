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
            Servings: 1,
            ServingUnit,
            (Cals * thisMultiplier) + (that.Cals * thatMultiplier),
            (P * thisMultiplier) + (that.P * thatMultiplier),
            (F * thisMultiplier) + (that.F * thatMultiplier),
            (CTotal * thisMultiplier) + (that.CTotal * thatMultiplier),
            (CFiber * thisMultiplier) + (that.CFiber * thatMultiplier));
    }

    public Macros Macros => new(P, F, CTotal - CFiber);

    public NutritionalInformation Convert(ServingUnit newServingUnit)
    {
        if (ServingUnit.UnitConversion.CentralUnit != newServingUnit.UnitConversion.CentralUnit)
        {
            throw new Exception($"{ServingUnit} has a different central unit than {newServingUnit}.");
        }

        var multiplier =  newServingUnit.UnitConversion.NumCentralUnitsInThisUnit /
            (Servings * ServingUnit.UnitConversion.NumCentralUnitsInThisUnit);

        return new(
            1, newServingUnit,
            Cals * multiplier,
            P * multiplier,
            F * multiplier,
            CTotal * multiplier,
            CFiber * multiplier);
    }
}
