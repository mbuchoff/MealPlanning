// ABOUTME: Represents an ordered chain of FoodGrouping options where each subsequent option
// is tried if the previous one fails to produce valid servings
namespace SystemOfEquations;

public record FallbackChain
{
    private readonly FoodGrouping[] _groupings;

    public FallbackChain(params FoodGrouping[] groupings)
    {
        if (groupings == null || groupings.Length < 2)
            throw new ArgumentException(
                "FallbackChain requires at least 2 food groupings",
                nameof(groupings));
        _groupings = groupings;
    }

    public FoodGrouping Primary => _groupings[0];
    public FoodGrouping[] Fallbacks => _groupings[1..];
    public FoodGrouping[] All => _groupings;
    public int Count => _groupings.Length;
}
