namespace SystemOfEquations;

internal record Helping(Food Food, double Servings)
{
    public override string ToString() => Food.ToString(Servings);

    public Macros Macros => Food.NutritionalInformation.Macros * Servings;
    public static Helping operator *(Helping h, double d) => new(h.Food, h.Servings * d);
}