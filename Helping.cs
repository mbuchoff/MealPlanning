namespace SystemOfEquations;

public record Helping(Food Food, double Servings)
{
    public override string ToString() => Food.ToString(Servings);

    public static Helping operator *(Helping h, double d) => new(h.Food, h.Servings * d);
}