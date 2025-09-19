# Seitan Food Separation Implementation Plan - TDD Approach

## Overview
Use Test-Driven Development to modify the system to output nutritional yeast and gluten as separate items in console and Todoist while maintaining combined macro calculations for the Seitan food serving.

## Current State
- `Seitan_Sprouts_Yeast_1_Gram_Gluten_4x` combines:
  - 1x Nutritional Yeast (16g)
  - 4x Gluten (30g each = 120g total)
- Ratio is 16g:120g (not true 4:1 by weight)
- Currently outputs as single item: "Seitan Walmart Nutritional Yeast, 4x gluten"
- Used in meal plans via `FoodGrouping` classes

## Proposed Solution

### Approach: Test-Driven Development with Composite FoodServing Pattern
1. Write failing tests first
2. Create minimal implementation to make tests compile
3. Implement functionality to make tests pass
4. Refactor if needed

### TDD Implementation Steps

#### Step 1: Write Failing Tests First
Create `Test/CompositeFoodServingTests.cs` with tests for:
- CompositeFoodServing ToString() outputs components on separate lines
- Scaling multiplies each component correctly
- Combined nutrition matches sum of components
- Water amount stays with composite only
- Integration with meal prep plans

Example test:
```csharp
[Test]
public void CompositeFoodServing_ToString_OutputsComponentsOnSeparateLines()
{
    // Arrange
    var yeast = new FoodServing("nutritional yeast",
        new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
    var gluten = new FoodServing("gluten",
        new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

    var composite = new CompositeFoodServing(
        "Seitan",
        new NutritionalInformation(1, ServingUnits.None, 79, 13.52M, 0.655M, 3.38M, 0.75M),
        [yeast, gluten],
        new FoodServing.AmountWater(0, 0.00916M));

    // Act
    var output = composite.ToString();

    // Assert
    Assert.That(output, Does.Contain("4.0g nutritional yeast"));
    Assert.That(output, Does.Contain("16.0g gluten"));
    Assert.That(output.Split('\n').Length, Is.EqualTo(2));
}

[Test]
public void CompositeFoodServing_Multiplication_ScalesComponents()
{
    // Arrange & Act
    var scaled = composite * 2;
    var output = scaled.ToString();

    // Assert
    Assert.That(output, Does.Contain("8.0g nutritional yeast"));
    Assert.That(output, Does.Contain("32.0g gluten"));
}
```

#### Step 2: Create Minimal Implementation (Red Phase)
Create `Executable/CompositeFoodServing.cs` with just enough structure to compile:
```csharp
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
}
```

Run tests to confirm they compile but fail.

#### Step 3: Implement Full Functionality (Green Phase)

1. **Complete CompositeFoodServing implementation:**
```csharp
public override string ToString()
{
    var scale = NutritionalInformation.ServingUnits;
    if (scale != 1)
    {
        return string.Join("\n", Components.Select(c => (c * scale).ToString()));
    }
    return string.Join("\n", Components.Select(c => c.ToString()));
}

public static CompositeFoodServing operator *(CompositeFoodServing fs, decimal multiplier) =>
    new(fs.Name,
        fs.NutritionalInformation * multiplier,
        fs.Components.Select(c => c * multiplier),
        fs.Water == null ? null : new AmountWater(fs.Water.Base, fs.Water.PerServing * multiplier));
```

2. **Update Foods.cs:**
```csharp
// Adjust to true 4:1 ratio (4g yeast : 16g gluten)
public static FoodServing Seitan_Sprouts_Yeast_1_Gram_Gluten_4x => new CompositeFoodServing(
    "Seitan (Nutritional Yeast + Gluten)",
    new NutritionalInformation(
        ServingUnits: 1,
        ServingUnit: ServingUnits.None,
        Cals: 60M * 0.25M + 120M * (16M/30M),  // 15 + 64 = 79
        P: 5M * 0.25M + 23M * (16M/30M),        // 1.25 + 12.27 = 13.52
        F: 0.5M * 0.25M + 1M * (16M/30M),       // 0.125 + 0.53 = 0.655
        CTotal: 5M * 0.25M + 4M * (16M/30M),    // 1.25 + 2.13 = 3.38
        CFiber: 3M * 0.25M + 0M * (16M/30M)),   // 0.75 + 0 = 0.75
    [
        NutritionalYeastFood.WithServing(4, ServingUnits.Gram),
        GlutenFood.WithServing(16, ServingUnits.Gram)
    ],
    water: new(Base: 0, PerServing: 0.00916M)); // Keep original water amount on composite
```

3. **Update FoodServingExtensions.cs:**
```csharp
// In CombineLikeServings method, preserve CompositeFoodServing type
if (first is CompositeFoodServing compositeFirst)
{
    var scaleFactor = totalServingUnits / first.NutritionalInformation.ServingUnits;
    return new CompositeFoodServing(
        first.Name,
        totalNutrition,
        compositeFirst.Components.Select(c => c * scaleFactor),
        first.Water);
}
```

4. **Update WeeklyMealsPrepPlan.cs:**
```csharp
// Handle composite foods in ToString()
var servingsStr = string.Join("\n", MealPrepPlans.SelectMany(m => m.Servings.SelectMany(s =>
{
    if (s is CompositeFoodServing composite)
    {
        return composite.Components.Select(c => $"{m.Name}: {c}");
    }
    return new[] { $"{m.Name}: {s}" };
})));
```

5. **Update TodoistService.cs:**
```csharp
private static async Task AddServingAsync(TodoistTask parentTodoistTask, FoodServing s)
{
    if (s is CompositeFoodServing composite)
    {
        foreach (var component in composite.Components)
        {
            await AddServingAsync(parentTodoistTask, component);
        }
    }
    else
    {
        // existing code...
    }
}
```

#### Step 4: Verify All Tests Pass
1. Run unit tests - all should pass
2. Run integration tests
3. Test actual program execution
4. Verify console output shows separated components
5. Verify Todoist integration (if applicable)

#### Step 5: Refactor (if needed)
- Clean up any duplication
- Improve naming/structure
- Ensure tests still pass

## Benefits of TDD Approach
1. **Confidence**: Tests ensure functionality works before implementation
2. **Clear requirements**: Tests document expected behavior
3. **Regression prevention**: Tests catch breaking changes
4. **Better design**: TDD often leads to cleaner, more modular code
5. **Documentation**: Tests serve as usage examples

## Resolved Design Decisions
1. **Water amount**: Stays with composite (not split to components)
2. **Display format**: Component names remain as defined in Foods.cs
3. **Ratio**: Adjusted to true 4:1 by weight (4g yeast : 16g gluten)
4. **Scope**: Only Seitan uses this pattern (no other composite foods)

## Testing Checklist
- [ ] Unit tests compile but fail initially (Red)
- [ ] All unit tests pass after implementation (Green)
- [ ] Console output shows yeast and gluten on separate lines
- [ ] Meal prep plan shows separated components
- [ ] Scaling works correctly (6.7x shows 26.8g yeast, 107.2g gluten)
- [ ] Water amount stays with composite
- [ ] Todoist creates separate subtasks for each component
- [ ] No regression in existing functionality

## Files to Create/Modify
1. **New**: `Test/CompositeFoodServingTests.cs`
2. **New**: `Executable/CompositeFoodServing.cs`
3. **Modified**: `Executable/Data/Foods.cs`
4. **Modified**: `Executable/FoodServingExtensions.cs`
5. **Modified**: `Executable/WeeklyMealsPrepPlan.cs`
6. **Modified**: `Executable/Todoist/TodoistService.cs`