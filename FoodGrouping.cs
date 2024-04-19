namespace SystemOfEquations;

internal record FoodGrouping
{
    internal FoodGrouping(string name, IList<Helping> staticHelpings, Food pFood, Food fFood, Food cFood)
    {
        Name = name;
        StaticHelpings = staticHelpings;
        PFood = pFood;
        FFood = fFood;
        CFood = cFood;
    }

    internal FoodGrouping(string name, Food pFood, Food fFood, Food cFood) : this(name, [], pFood, fFood, cFood)
    {

    }

    public string Name { get; }
    public IList<Helping> StaticHelpings { get; }
    public Food PFood { get; }
    public Food FFood { get; }
    public Food CFood { get; }

    public override string ToString() => Name;
}
