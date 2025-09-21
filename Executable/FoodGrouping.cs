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

    public static FoodGrouping operator +(FoodGrouping grouping, FoodServing food)
    {
        var newStaticServings = grouping.StaticServings.ToList();
        newStaticServings.Add(food);

        return new FoodGrouping(
            grouping.Name,
            newStaticServings,
            grouping.PFood,
            grouping.FFood,
            grouping.CFood,
            grouping.PreparationMethod);
    }

    public static FoodGrouping operator +(FoodGrouping grouping, (FoodServing food, decimal servings) item)
    {
        var newStaticServings = grouping.StaticServings.ToList();
        newStaticServings.Add(item.food * item.servings);

        return new FoodGrouping(
            grouping.Name,
            newStaticServings,
            grouping.PFood,
            grouping.FFood,
            grouping.CFood,
            grouping.PreparationMethod);
    }
}
