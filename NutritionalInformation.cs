namespace SystemOfEquations;

internal record NutritionalInformation(double Cals, Macros Macros)
{
    public static NutritionalInformation operator +(NutritionalInformation n1, NutritionalInformation n2) =>
        new(n1.Cals + n2.Cals, n1.Macros + n2.Macros);
    public static NutritionalInformation operator *(NutritionalInformation n, double d) =>
        new(n.Cals * d, n.Macros * d);
}
