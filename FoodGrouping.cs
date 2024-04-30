namespace SystemOfEquations;

internal record FoodGrouping
{
    internal FoodGrouping(
        string name,
        IList<Helping> staticHelpings,
        Food pFood,
        Food fFood,
        Food cFood,
        PreparationMethodEnum preparationMethod)
    {
        Name = name;
        StaticHelpings = staticHelpings;
        PFood = pFood;
        FFood = fFood;
        CFood = cFood;
        PreparationMethod = preparationMethod;
    }

    internal FoodGrouping(string name, Food pFood, Food fFood, Food cFood, PreparationMethodEnum preparationMethod) :
        this(name, [], pFood, fFood, cFood, preparationMethod)
    {

    }

    public string Name { get; }
    public IList<Helping> StaticHelpings { get; }
    public Food PFood { get; }
    public Food FFood { get; }
    public Food CFood { get; }

    public enum PreparationMethodEnum { PrepareInAdvance, PrepareAsNeeded }
    public PreparationMethodEnum PreparationMethod { get; }

    public override string ToString() => Name;
}
