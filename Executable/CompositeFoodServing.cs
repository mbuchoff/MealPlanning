using SystemOfEquations.Data;

namespace SystemOfEquations;

public record CompositeFoodServing : FoodServing
{
    public IReadOnlyList<FoodServing> Components { get; }

    public CompositeFoodServing(
        string name,
        NutritionalInformation combinedNutrition,
        IEnumerable<FoodServing> components)
        : base(name, combinedNutrition)
    {
        Components = components.ToList().AsReadOnly();
    }

    // Factory method that auto-calculates nutrition from components
    public static CompositeFoodServing FromComponents(
        string name,
        IEnumerable<FoodServing> components)
    {
        var componentsList = components.ToList();
        var combinedNutrition = componentsList
            .Select(c => c.NutritionalInformation)
            .Sum(1, ServingUnits.None);

        return new CompositeFoodServing(name, combinedNutrition, componentsList);
    }

    // Factory method that creates a composite with mixed scalable and static components
    public static CompositeFoodServing FromComponentsWithStatic(
        string name,
        IEnumerable<FoodServing> scalableComponents,
        IEnumerable<FoodServing> staticComponents)
    {
        var scalableList = scalableComponents.ToList();
        var staticList = staticComponents.Select(c => new StaticFoodServing(c)).ToList();

        // Combine all components
        var allComponents = new List<FoodServing>();
        allComponents.AddRange(scalableList);
        allComponents.AddRange(staticList);

        // Calculate combined nutrition from all components
        var combinedNutrition = allComponents
            .Select(c => c.NutritionalInformation)
            .Sum(1, ServingUnits.None);

        return new CompositeFoodServing(name, combinedNutrition, allComponents);
    }

    // Combines components with the same name and base unit
    private static IEnumerable<FoodServing> CombineLikeComponents(IEnumerable<FoodServing> components)
    {
        return components
            .GroupBy(c => new
            {
                c.Name,
                BaseUnit = c.NutritionalInformation.ServingUnit
            })
            .Select(group => group.Count() == 1
                ? group.First()
                : CombineComponents(group));
    }

    private static FoodServing CombineComponents(IGrouping<dynamic, FoodServing> group)
    {
        var totalUnits = group.Sum(c => c.NutritionalInformation.ServingUnits);
        var firstComponent = group.First();

        return firstComponent with
        {
            NutritionalInformation = firstComponent.NutritionalInformation with
            {
                ServingUnits = totalUnits
            }
        };
    }

    // Override ToString to output components with combination
    public override string ToString()
    {
        // Check if we've been scaled (servings != 1)
        var scale = NutritionalInformation.ServingUnits;
        if (scale != 1)
        {
            // Output scaled components using polymorphic ApplyScale method
            var scaledComponents = Components.Select(c => c.ApplyScale(scale));
            var combinedComponents = CombineLikeComponents(scaledComponents);
            return string.Join("\n", combinedComponents.Select(c => c.ToString()));
        }

        // Output components as-is for unscaled, but still combine them
        var unscaledCombinedComponents = CombineLikeComponents(Components);
        return string.Join("\n", unscaledCombinedComponents.Select(c => c.ToString()));
    }

    // Override to return scaled components for display
    public override IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        var scale = NutritionalInformation.ServingUnits;

        // Get scaled components using polymorphic ApplyScale method
        var scaledComponents = Components.Select(c => c.ApplyScale(scale));

        // Combine like components
        var combinedComponents = CombineLikeComponents(scaledComponents);
        foreach (var component in combinedComponents)
        {
            yield return component;
        }
    }

    // Note: Multiplication is handled in base FoodServing class to preserve type
}