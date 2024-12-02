namespace SystemOfEquations;

internal record FoodGrouping
{
    internal FoodGrouping(
        string name,
        IList<Helping> staticHelpings,
        Food pFood, ServingUnit pServingUnit,
        Food fFood, ServingUnit fServingUnit,
        Food cFood, ServingUnit cServingUnit,
        PreparationMethodEnum preparationMethod)
    {
        Name = name;
        StaticHelpings = staticHelpings;
        PFood = pFood;
        PServingUnit = pServingUnit;
        FFood = fFood;
        FServingUnit = fServingUnit;
        CFood = cFood;
        CServingUnit = cServingUnit;
        PreparationMethod = preparationMethod;
    }

    internal FoodGrouping(
        string name,
        Food pFood, ServingUnit pServingUnit,
        Food fFood, ServingUnit fServingUnit,
        Food cFood, ServingUnit cServingUnit,
        PreparationMethodEnum preparationMethod) :
            this(name, [], pFood, pServingUnit, fFood, fServingUnit, cFood, cServingUnit, preparationMethod)
    {

    }

    public string Name { get; }
    public IList<Helping> StaticHelpings { get; }
    public Food PFood { get; }
    public ServingUnit PServingUnit { get; }
    public Food FFood { get; }
    public ServingUnit FServingUnit { get; }
    public Food CFood { get; }
    public ServingUnit CServingUnit { get; }

    public enum PreparationMethodEnum { PrepareInAdvance, PrepareAsNeeded }
    public PreparationMethodEnum PreparationMethod { get; }

    public override string ToString() => Name;
}
