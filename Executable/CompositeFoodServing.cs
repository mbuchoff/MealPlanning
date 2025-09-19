namespace SystemOfEquations;

public record CompositeFoodServing : FoodServing
{
    public IReadOnlyList<FoodServing> Components { get; }

    public CompositeFoodServing(
        string name,
        NutritionalInformation combinedNutrition,
        IEnumerable<FoodServing> components,
        AmountWater? water = null)
        : base(name, combinedNutrition, water)
    {
        Components = components.ToList().AsReadOnly();
    }

    // Override ToString to output components
    public override string ToString()
    {
        // Check if we've been scaled (servings != 1)
        var scale = NutritionalInformation.ServingUnits;
        if (scale != 1)
        {
            // Output scaled components
            return string.Join("\n", Components.Select(c => (c * scale).ToString()));
        }
        // Output components as-is for unscaled
        return string.Join("\n", Components.Select(c => c.ToString()));
    }

    // Override to return component lines instead of composite line
    public override IEnumerable<string> ToOutputLines(string prefix = "")
    {
        var scale = NutritionalInformation.ServingUnits;
        return Components.Select(c => $"{prefix}{c * scale}");
    }

    // Override to return scaled components for display
    public override IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        var scale = NutritionalInformation.ServingUnits;
        return Components.Select(c => c * scale);
    }

    // Note: Multiplication is handled in base FoodServing class to preserve type
}