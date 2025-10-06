namespace SystemOfEquations;

/// <summary>
/// A wrapper around FoodServing that prevents scaling when multiplied.
/// Static components maintain their fixed serving amounts regardless of composite serving multipliers.
/// </summary>
public record StaticFoodServing : FoodServing
{
    /// <summary>
    /// The original FoodServing that this static wrapper preserves
    /// </summary>
    public FoodServing OriginalServing { get; }

    public StaticFoodServing(FoodServing originalServing)
        : base(originalServing.Name, originalServing.NutritionalInformation, originalServing.IsConversion)
    {
        OriginalServing = originalServing;
    }

    /// <summary>
    /// Override multiplication to return the unchanged instance.
    /// This is the key behavior that makes components "static" - they don't scale.
    /// </summary>
    public static StaticFoodServing operator *(StaticFoodServing staticServing, decimal multiplier) =>
        staticServing; // Return unchanged - this is what makes it "static"

    /// <summary>
    /// Override ApplyScale to return unchanged instance.
    /// This provides polymorphic scaling behavior without type checking.
    /// </summary>
    public override FoodServing ApplyScale(decimal scale) => this;

    /// <summary>
    /// Override ToString to use the original serving's formatting
    /// </summary>
    public override string ToString() => OriginalServing.ToString();

    /// <summary>
    /// Override ToOutputLines to use the original serving's formatting
    /// </summary>
    public override IEnumerable<string> ToOutputLines(string prefix = "")
    {
        return OriginalServing.ToOutputLines(prefix);
    }

    /// <summary>
    /// Override GetComponentsForDisplay to return the original serving
    /// </summary>
    public override IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        return OriginalServing.GetComponentsForDisplay();
    }
}