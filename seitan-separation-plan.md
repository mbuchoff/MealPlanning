# Seitan Food Separation Implementation Plan

## Overview
Modify the system to output nutritional yeast and gluten as separate items in console and Todoist while maintaining combined macro calculations for the Seitan food serving. All changes will be focused in the FoodServing-related files with minimal impact to other classes.

## Current State
- `Seitan_Sprouts_Yeast_1_Gram_Gluten_4x` combines:
  - 1x Nutritional Yeast (16g)
  - 4x Gluten (30g each = 120g total)
- Ratio is 16g:120g (not true 4:1 by weight)
- Currently outputs as single item: "Seitan Walmart Nutritional Yeast, 4x gluten"
- Used in meal plans via `FoodGrouping` classes

## Proposed Solution

### Approach: Composite FoodServing Pattern
Create a new `CompositeFoodServing` class that:
1. Inherits from `FoodServing`
2. Stores constituent food servings internally
3. Overrides `ToString()` to output constituents separately
4. Maintains combined nutritional information for calculations
5. Handles scaling correctly when multiplied

### Implementation Steps

#### 1. Create CompositeFoodServing Class (New file: Executable/CompositeFoodServing.cs)
```csharp
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

    // Override ToString to return components separately
    public override string ToString() => ToString(1);

    public new string ToString(decimal quantity)
    {
        return string.Join("\n", Components.Select(c => (c * quantity).ToString()));
    }

    // Override multiplication to scale components
    public static CompositeFoodServing operator *(CompositeFoodServing fs, decimal multiplier) =>
        new(fs.Name,
            fs.NutritionalInformation * multiplier,
            fs.Components.Select(c => c * multiplier),
            fs.Water == null ? null : new AmountWater(fs.Water.Base, fs.Water.PerServing * multiplier));
}
```

#### 2. Update Foods.cs
Change the Seitan definition to:
```csharp
// Adjust to true 4:1 ratio (4g yeast : 16g gluten)
public static FoodServing Seitan_Sprouts_Yeast_1_Gram_Gluten_4x => new CompositeFoodServing(
    "Seitan (Nutritional Yeast + Gluten)",
    // Combined nutrition (manually calculated or using Combine method)
    NutritionalYeast_Sprouts_16_Grams.NutritionalInformation * 0.25M +  // 4g yeast
    Gluten_30_Grams.NutritionalInformation * (16M/30M),                  // 16g gluten
    // Components
    [
        NutritionalYeastFood.WithServing(4, ServingUnits.Gram),
        GlutenFood.WithServing(16, ServingUnits.Gram)
    ],
    Water: new(Base: 0, PerServing: 0.0366666666667M * 0.25M)); // Adjust water proportionally
```

#### 3. Update TodoistService.cs
Modify `AddServingAsync` to handle composite foods:
```csharp
private static async Task AddServingAsync(TodoistTask parentTodoistTask, FoodServing s)
{
    if (s is CompositeFoodServing composite)
    {
        // Add each component as a separate subtask
        foreach (var component in composite.Components)
        {
            await AddServingAsync(parentTodoistTask, component);
        }
    }
    else
    {
        Console.WriteLine($"Adding subtask {parentTodoistTask.Content} > {s}...");
        await AddTaskAsync(
            s.ToString(), description: null, dueString: null, parentTodoistTask.Id, projectId: null);
        Console.WriteLine($"Added subtask {parentTodoistTask.Content} > {s}");
    }
}
```

#### 4. Minimal Changes to Meal.cs
Since `CompositeFoodServing` overrides `ToString()`, the existing `Meal.ToString()` should automatically handle the separation when it calls `serving.ToString()`.

## Benefits of This Approach
1. **Minimal changes**: Only FoodServing-related files affected
2. **Backward compatible**: Other foods continue to work unchanged
3. **Reusable**: Can apply to other composite foods if needed
4. **Clean separation**: Output logic stays in FoodServing classes
5. **Proper scaling**: Components scale correctly with meal prep quantities

## Questions to Resolve
1. **Water amount**: Should the 0.0366666666667M cups water per serving be associated with gluten only or split?
2. **Display format**: Should component names include their source (e.g., "nutritional yeast from Sprouts")?
3. **Ratio confirmation**: Is 4g yeast + 16g gluten the desired 4:1 ratio?
4. **Future composites**: Should this pattern be applied to any other food combinations?

## Testing Checklist
- [ ] Console output shows yeast and gluten as separate lines
- [ ] Todoist creates separate subtasks for each component
- [ ] Macro calculations remain correct
- [ ] Meal prep scaling works correctly (components scale proportionally)
- [ ] Water amounts display correctly if applicable