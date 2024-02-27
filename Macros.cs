namespace SystemOfEquations;

public record Macros(double P, double F, double C)
{
    public static Macros operator -(Macros m1, Macros m2) => new(m1.P - m2.P, m1.F - m2.F, m1.C - m2.C);
    public static Macros operator +(Macros m1, Macros m2) => new(m1.P + m2.P, m1.F + m2.F, m1.C + m2.C);
    public static Macros operator *(Macros m, double d) => new(m.P * d, m.F * d, m.C * d);
}
