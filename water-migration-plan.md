# Water Migration Plan: From AmountWater to Composite Food Pattern

## Overview

Migrate from the current special `AmountWater` parameter system to using the composite food pattern with static/scalable components. This will unify water handling with the existing static serving component architecture.

## Current System Analysis

### Current Water Implementation
```csharp
// Current: Special AmountWater record
public record AmountWater(decimal Base, decimal PerServing)

// Current: Foods with water
new Food("brown rice", [...], nutrition, Water: new(Base: 1.5M, PerServing: 0.5M))

// Current: Display logic
$", {Water.Base + Water.PerServing * quantity:f1} cups water"
```

### Foods Currently Using AmountWater
1. **BrownRice**: `Water: new(Base: 1.5M, PerServing: 0.5M)`
2. **Farro**: `Water: new(Base: 0, PerServing: 1.333333333332M)`
3. **PearledBarley**: `Water: new(Base: 1.5M, PerServing: 0.6M)`
4. **QueatBerries**: `Water: new(Base: 2, PerServing: 0.6M)`
5. **Quinoa**: `Water: new(Base: 1.5M, PerServing: 0.6M)`
6. **ToastedWheatfuls**: `Water: new(Base: 2, PerServing: 0.6M)`

## Target System Design

### New Water Implementation
```csharp
// New: Regular water food
private static Food WaterFood { get; } = new("water",
    [(1, ServingUnits.Cup)],
    new BaseNutrition(Cals: 0, P: 0, F: 0, CTotal: 0, CFiber: 0));

// New: Composite foods with water components
CompositeFoodServing.FromComponentsWithStatic(
    "brown rice",
    scalableComponents: [
        BrownRiceBaseFood.WithServing(45, ServingUnits.Gram),
        WaterFood.WithServing(0.5M, ServingUnits.Cup) // Per-serving water
    ],
    staticComponents: [
        new StaticFoodServing(WaterFood.WithServing(1.5M, ServingUnits.Cup)) // Base water
    ]);

// New: Automatic component combination in display
"90 grams brown rice, 2.5 cups water" // Combines 1.5 + 1.0 cups
```

## TDD Implementation Plan

### Phase 1: Foundation and Tests

#### 1.1 Create WaterMigrationTests.cs
```csharp
public class WaterMigrationTests
{
    [Fact]
    public void Water_Food_Should_Have_Zero_Macros()

    [Fact]
    public void StaticWater_Should_Not_Scale_When_Composite_Multiplied()

    [Fact]
    public void ScalableWater_Should_Scale_When_Composite_Multiplied()

    [Fact]
    public void Combined_Water_Display_Should_Match_Old_AmountWater_Output()

    [Fact]
    public void Generic_Component_Combination_Should_Work_For_Any_Food()
}
```

#### 1.2 Test Water Migration for Each Food
```csharp
[Theory]
[InlineData("BrownRice", 1.5, 0.5)] // Base: 1.5, PerServing: 0.5
[InlineData("PearledBarley", 1.5, 0.6)]
[InlineData("QueatBerries", 2, 0.6)]
public void MigratedFood_Should_Have_Equivalent_Water_Amounts(
    string foodName, decimal expectedBase, decimal expectedPerServing)
```

### Phase 2: Component Combination Logic

#### 2.1 Implement Generic Combination
```csharp
// In CompositeFoodServing or extension method
private static IEnumerable<FoodServing> CombineLikeComponents(
    IEnumerable<FoodServing> components)
{
    return components
        .GroupBy(c => new {
            c.Name,
            BaseUnit = c.NutritionalInformation.ServingUnit.UnitConversion.CentralUnit
        })
        .Select(group => group.Count() == 1
            ? group.First()
            : CombineComponents(group));
}

private static FoodServing CombineComponents(IGrouping<object, FoodServing> group)
{
    var totalUnits = group.Sum(c => c.NutritionalInformation.ServingUnits);
    var firstComponent = group.First();

    return firstComponent with {
        NutritionalInformation = firstComponent.NutritionalInformation with {
            ServingUnits = totalUnits
        }
    };
}
```

#### 2.2 Update Display Methods
```csharp
// Update ToString() and ToOutputLines() to use combination logic
public override string ToString()
{
    var scale = NutritionalInformation.ServingUnits;
    if (scale != 1)
    {
        var scaledComponents = GetScaledComponents();
        var combinedComponents = CombineLikeComponents(scaledComponents);
        return string.Join("\n", combinedComponents.Select(c => c.ToString()));
    }

    var combinedComponents = CombineLikeComponents(Components);
    return string.Join("\n", combinedComponents.Select(c => c.ToString()));
}
```

### Phase 3: Food Migration (One by One)

#### 3.1 BrownRice Migration Pattern
```csharp
// Before:
private static Food BrownRiceFood { get; } = new("brown rice",
    [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
    new BaseNutrition(Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2),
    Water: new(Base: 1.5M, PerServing: 0.5M));

// After:
private static Food BrownRiceBaseFood { get; } = new("brown rice",
    [(45, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
    new BaseNutrition(Cals: 170, P: 4, F: 1.5M, CTotal: 35, CFiber: 2));

public static FoodServing BrownRice_45_Grams => CompositeFoodServing.FromComponentsWithStatic(
    "brown rice",
    scalableComponents: [
        BrownRiceBaseFood.WithServing(45, ServingUnits.Gram),
        WaterFood.WithServing(0.5M, ServingUnits.Cup)
    ],
    staticComponents: [
        new StaticFoodServing(WaterFood.WithServing(1.5M, ServingUnits.Cup))
    ]);
```

#### 3.2 Migration Order
1. **BrownRice** (Base: 1.5, PerServing: 0.5) - Most commonly used
2. **Farro** (Base: 0, PerServing: 1.33) - No static component
3. **PearledBarley** (Base: 1.5, PerServing: 0.6) - Standard pattern
4. **Quinoa** (Base: 1.5, PerServing: 0.6) - Standard pattern
5. **ToastedWheatfuls** (Base: 2, PerServing: 0.6) - Larger base
6. **QueatBerries** (Base: 2, PerServing: 0.6) - Largest amounts

### Phase 4: System Cleanup

#### 4.1 Remove AmountWater System
```csharp
// Remove from FoodServing.cs:
public record AmountWater(decimal Base, decimal PerServing) // DELETE
FoodServing.AmountWater? Water = null, // DELETE from constructor

// Remove from Food.cs:
FoodServing.AmountWater? Water = null, // DELETE from constructor

// Remove from CompositeFoodServing.cs:
AmountWater? water = null // DELETE from all constructors

// Remove special water handling in:
- FoodServing.ToString()
- FoodServing.operator *
- CompositeFoodServing.ToOutputLines()
- CompositeFoodServing.GetComponentsForDisplay()
```

#### 4.2 Update Method Signatures
Remove `water` parameters from all public APIs while maintaining backward compatibility where possible.

### Phase 5: Testing and Verification

#### 5.1 Backward Compatibility Tests
```csharp
[Fact]
public void Migration_Should_Produce_Identical_Water_Amounts()
{
    // Test each migrated food produces same total water as before
    var testCases = new[]
    {
        (Food: "BrownRice", Multiplier: 1M, Expected: 2.0M),  // 1.5 + 0.5
        (Food: "BrownRice", Multiplier: 2M, Expected: 2.5M),  // 1.5 + 1.0
        (Food: "QueatBerries", Multiplier: 1M, Expected: 2.6M), // 2.0 + 0.6
    };

    foreach (var test in testCases)
    {
        var food = GetMigratedFood(test.Food) * test.Multiplier;
        var totalWater = CalculateTotalWater(food);
        Assert.Equal(test.Expected, totalWater);
    }
}
```

#### 5.2 Integration Tests
```csharp
[Fact]
public void Meal_Planning_Output_Should_Be_Identical()
{
    // Compare meal planning output before/after migration
    // Ensure water shows up correctly in totals
}

[Fact]
public void Todoist_Integration_Should_Work_With_Combined_Water()
{
    // Test that Todoist shows combined water amounts correctly
}
```

## Key Design Decisions

### 1. Automatic Component Combination
- **Where**: In display methods (`ToString()`, `ToOutputLines()`)
- **How**: Group by name and base unit, sum serving amounts
- **Why**: User requested combined display, generic for any food type

### 2. Water as Zero-Macro Food
- **Nutrition**: All zeros (calories, protein, fat, carbs, fiber)
- **Unit**: Cups (matches current display)
- **Name**: Simply "water"

### 3. Static vs Scalable Water Pattern
- **Static Water**: Base cooking water that doesn't scale (e.g., 1.5 cups to start)
- **Scalable Water**: Absorption water that scales with servings (e.g., 0.5 cups per serving)
- **Implementation**: Use `FromComponentsWithStatic` pattern

### 4. Migration Strategy
- **Test-first**: All tests written before implementation
- **One-by-one**: Migrate foods individually with verification
- **Backward compatibility**: Ensure identical behavior
- **Clean removal**: Delete AmountWater system only after full migration

## Benefits After Migration

### 1. Architectural Consistency
- Water uses same pattern as seasonings/ingredients
- No special-case handling for water
- Unified component system

### 2. Enhanced Flexibility
- Multiple static water components possible
- Multiple scalable water components possible
- Different water types in future (flavored, etc.)

### 3. Simplified Codebase
- Removes AmountWater record and all special handling
- Cleaner method signatures
- Less complexity in scaling and display logic

### 4. Better User Experience
- Combined water display (e.g., "2.5 cups water" instead of separate amounts)
- Consistent component handling
- More intuitive food composition

## Potential Challenges

### 1. Complex Water Relationships
- **QueatBerries**: Largest water amounts, good stress test
- **Farro**: No base water (only scalable), edge case

### 2. Display Format Changes
- Need to ensure decimal precision maintained
- Component ordering might change
- Todoist integration needs testing

### 3. Performance Considerations
- Component combination logic adds processing
- Grouping and summing operations
- Should benchmark with large composite foods

## Success Criteria

1. ✅ All existing tests pass after migration
2. ✅ Water amounts are mathematically identical to old system
3. ✅ Display output shows combined water amounts
4. ✅ No performance regressions
5. ✅ Todoist integration works correctly
6. ✅ Meal planning output is visually identical
7. ✅ Generic combination works for other food types

## Timeline Estimate

- **Phase 1-2** (Foundation): 2-3 hours
- **Phase 3** (Migration): 3-4 hours
- **Phase 4** (Cleanup): 1-2 hours
- **Phase 5** (Testing): 2-3 hours
- **Total**: 8-12 hours of development time

This plan ensures a safe, test-driven migration from the current AmountWater system to a unified composite food component architecture.