using SystemOfEquations.Data;

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

    // Factory method that auto-calculates nutrition from components
    public static CompositeFoodServing FromComponents(
        string name,
        IEnumerable<FoodServing> components,
        AmountWater? water = null)
    {
        var componentsList = components.ToList();
        var combinedNutrition = componentsList
            .Select(c => c.NutritionalInformation)
            .Sum(1, ServingUnits.None);

        return new CompositeFoodServing(name, combinedNutrition, componentsList, water);
    }

    // Factory method that creates a composite with mixed scalable and static components
    public static CompositeFoodServing FromComponentsWithStatic(
        string name,
        IEnumerable<FoodServing> scalableComponents,
        IEnumerable<FoodServing> staticComponents,
        AmountWater? water = null)
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

        return new CompositeFoodServing(name, combinedNutrition, allComponents, water);
    }

    // Override ToString to output components
    public override string ToString()
    {
        // Check if we've been scaled (servings != 1)
        var scale = NutritionalInformation.ServingUnits;
        if (scale != 1)
        {
            // Output scaled components, preserving static components unchanged
            return string.Join("\n", Components.Select(c =>
            {
                if (c is StaticFoodServing staticComponent)
                {
                    return staticComponent.ToString(); // Use unchanged static component
                }
                else
                {
                    return (c * scale).ToString(); // Scale regular components
                }
            }));
        }
        // Output components as-is for unscaled
        return string.Join("\n", Components.Select(c => c.ToString()));
    }

    // Override to return component lines instead of composite line
    public override IEnumerable<string> ToOutputLines(string prefix = "")
    {
        var scale = NutritionalInformation.ServingUnits;

        // Output scaled components, but preserve static components unchanged
        foreach (var component in Components)
        {
            // Check if this is a static component - if so, don't scale it
            if (component is StaticFoodServing staticComponent)
            {
                yield return $"{prefix}{staticComponent}"; // Use unchanged static component
            }
            else
            {
                yield return $"{prefix}{component * scale}"; // Scale regular components
            }
        }

        // If there's water, add it as an output line
        if (Water != null)
        {
            // Water.PerServing is already scaled when the composite is multiplied
            var waterAmount = Water.Base + Water.PerServing;
            if (waterAmount > 0.01M) // Only show if meaningful amount
            {
                yield return $"{prefix}{waterAmount:f1} cups water";
            }
        }
    }

    // Override to return scaled components for display
    public override IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        var scale = NutritionalInformation.ServingUnits;

        // Return scaled components, but preserve static components unchanged
        foreach (var component in Components)
        {
            // Check if this is a static component - if so, don't scale it
            if (component is StaticFoodServing staticComponent)
            {
                yield return staticComponent; // Return unchanged
            }
            else
            {
                yield return component * scale; // Scale regular components
            }
        }

        // If there's water, add it as a virtual component
        if (Water != null)
        {
            // Water.PerServing is already scaled when the composite is multiplied
            // So we just use it directly with quantity=1 (since this is for display of the current serving)
            var waterAmount = Water.Base + Water.PerServing;
            // Create a water serving using Cup as the unit, with waterAmount as the serving size
            var waterServing = new FoodServing(
                "water",
                new NutritionalInformation(waterAmount, ServingUnits.Cup, 0, 0, 0, 0, 0),
                null,
                false);
            yield return waterServing;
        }
    }

    // Override to create parent task for composite and subtasks for components
    public override async Task<string?> CreateTodoistSubtasksAsync(
        string parentTaskId,
        Func<string, string?, string?, string?, string?, Task<object>> addTaskFunc)
    {
        // Create a parent task for the composite food
        Console.WriteLine($"Adding subtask > {Name}...");
        var compositeTask = await addTaskFunc(Name, null, null, parentTaskId, null);
        Console.WriteLine($"Added subtask > {Name}");

        // Extract task ID from the returned object
        var taskId = compositeTask.GetType().GetProperty("Id")?.GetValue(compositeTask)?.ToString();
        if (taskId == null)
            throw new InvalidOperationException("Could not get task ID from created task");

        // Add each component as a subtask of the composite
        foreach (var component in GetComponentsForDisplay())
        {
            await component.CreateTodoistSubtasksAsync(taskId, addTaskFunc);
        }

        return taskId;
    }

    // Note: Multiplication is handled in base FoodServing class to preserve type
}