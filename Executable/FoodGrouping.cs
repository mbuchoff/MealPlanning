namespace SystemOfEquations;

public record FoodGrouping
{
    public FoodGrouping(
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

    public FoodGrouping(string name, Food pFood, Food fFood, Food cFood, PreparationMethodEnum preparationMethod) :
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
    
    public FoodGrouping AddStaticFood(Food food, int servings = 1)
    {
        var newStaticHelpings = StaticHelpings.ToList();
        newStaticHelpings.Add(new Helping(food, servings));
        
        return new FoodGrouping(
            Name,
            newStaticHelpings,
            PFood,
            FFood,
            CFood,
            PreparationMethod);
    }
    
    public FoodGrouping AddStaticFood(params Helping[] helpings)
    {
        var newStaticHelpings = StaticHelpings.ToList();
        newStaticHelpings.AddRange(helpings);
        
        return new FoodGrouping(
            Name,
            newStaticHelpings,
            PFood,
            FFood,
            CFood,
            PreparationMethod);
    }
}
