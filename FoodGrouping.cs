namespace SystemOfEquations;

internal record FoodGrouping
{
    internal FoodGrouping(IList<Helping> staticHelpings, Food pFood, Food fFood, Food cFood)
    {
        StaticHelpings = staticHelpings;
        PFood = pFood;
        FFood = fFood;
        CFood = cFood;
    }

    internal FoodGrouping(Food pFood, Food fFood, Food cFood) : this([], pFood, fFood, cFood)
    {

    }

    public IList<Helping> StaticHelpings { get; }
    public Food PFood { get; }
    public Food FFood { get; }
    public Food CFood { get; }
}
