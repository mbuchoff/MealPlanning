namespace SystemOfEquations;

public record FoodGrouping
{
    public FoodGrouping(
        string name,
        IList<FoodServing> staticServings,
        FoodServing pFood,
        FoodServing fFood,
        FoodServing cFood,
        PreparationMethodEnum preparationMethod)
    {
        Name = name;
        StaticServings = staticServings;
        PFood = pFood;
        FFood = fFood;
        CFood = cFood;
        PreparationMethod = preparationMethod;
    }

    public FoodGrouping(string name, FoodServing pFood, FoodServing fFood, FoodServing cFood, PreparationMethodEnum preparationMethod) :
        this(name, [], pFood, fFood, cFood, preparationMethod)
    {

    }

    public string Name { get; }
    public IList<FoodServing> StaticServings { get; }
    public FoodServing PFood { get; }
    public FoodServing FFood { get; }
    public FoodServing CFood { get; }

    public enum PreparationMethodEnum { PrepareInAdvance, PrepareAsNeeded }
    public PreparationMethodEnum PreparationMethod { get; }

    public override string ToString() => Name;
    
    public FoodGrouping AddStaticFood(FoodServing food, decimal servings = 1)
    {
        var newStaticServings = StaticServings.ToList();
        newStaticServings.Add(food * servings);
        
        return new FoodGrouping(
            Name,
            newStaticServings,
            PFood,
            FFood,
            CFood,
            PreparationMethod);
    }
    
    public FoodGrouping AddStaticFood(params FoodServing[] servings)
    {
        var newStaticServings = StaticServings.ToList();
        newStaticServings.AddRange(servings);
        
        return new FoodGrouping(
            Name,
            newStaticServings,
            PFood,
            FFood,
            CFood,
            PreparationMethod);
    }
}
