namespace SystemOfEquations;

/// <summary>
/// Represents base nutritional information independent of serving size.
/// These values get scaled based on the actual serving amount.
/// </summary>
public record BaseNutrition(
    decimal Cals,
    decimal P,
    decimal F,
    decimal CTotal,
    decimal CFiber)
{
    public Macros Macros => new(P, F, CTotal - CFiber);
}