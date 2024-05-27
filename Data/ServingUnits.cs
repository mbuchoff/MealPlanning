﻿namespace SystemOfEquations.Data;

internal static class ServingUnits
{
    public static ServingUnit BlockTofu = new("block", decimalsToDisplay: 2);
    public static ServingUnit Cup = new("cup", decimalsToDisplay: 1);
    public static ServingUnit Gram = new("gram", decimalsToDisplay: 0);
    public static ServingUnit Scoop = new("scoop", decimalsToDisplay: 1, unitConversion: (0.380408, Unit: Cup));
    public static ServingUnit Tablespoon = new("tbsp", decimalsToDisplay: 1, unitConversion: (1.0/16, Cup));
}
