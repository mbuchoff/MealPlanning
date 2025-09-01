# Food Class Refactoring Plan

## Problem Statement

The current codebase has redundancy in food definitions. Foods like pumpkin seeds are defined multiple times for different serving sizes:
- `PumpkinSeeds_30_Grams`
- `PumpkinSeeds_1_Scoop`

Both represent the same food, just in different quantities. This creates maintenance issues and conceptual confusion - pumpkin seeds are pumpkin seeds regardless of serving size.

## Solution Overview

Separate the concept of a **food type** from its **serving size** by introducing a clearer class hierarchy:

1. **Food** - Represents the food itself with its nutritional equivalences
2. **FoodServing** - Represents a specific portion of that food
3. **BaseNutrition** - Stores nutritional values that get scaled

## Detailed Design

### New Class Structure

#### BaseNutrition.cs
```csharp
public record BaseNutrition(
    decimal Cals,
    decimal P,
    decimal F,
    decimal CTotal,
    decimal CFiber)
{
    public Macros Macros => new(P, F, CTotal - CFiber);
}
```

#### Food.cs (new)
```csharp
public record Food(
    string Name,
    (decimal Amount, ServingUnit Unit)[] ServingEquivalences,
    BaseNutrition Nutrition,
    FoodServing.AmountWater? Water = null,
    bool IsConversion = false)
{
    public FoodServing WithServing(decimal amount, ServingUnit unit)
    {
        // Creates a FoodServing with scaled nutrition
    }
}
```

#### FoodServing.cs (renamed from Food.cs, consolidates Helping.cs)
```csharp
public record FoodServing(
    string Name,
    NutritionalInformation NutritionalInformation,
    FoodServing.AmountWater? Water = null,
    bool IsConversion = false)
{
    // All existing Food methods
    // Plus multiplication operator from Helping
    public static FoodServing operator *(FoodServing fs, decimal multiplier)
}
```

### Foods.cs Structure

Before:
```csharp
public static Food PumpkinSeeds_30_Grams { get; } = new Food("pumpkin seeds", new(
    30, ServingUnits.Gram, Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));

public static Food PumpkinSeeds_1_Scoop { get; } = PumpkinSeeds_30_Grams
    .Copy(ServingUnits.Cup, newServings: 0.25M).Convert(ServingUnits.Scoop);
```

After:
```csharp
// Base food definition (private)
private static Food PumpkinSeeds { get; } = new("pumpkin seeds",
    [(30, ServingUnits.Gram), (0.25M, ServingUnits.Cup)],
    new BaseNutrition(Cals: 170, P: 9, F: 15, CTotal: 3, CFiber: 2));

// Specific servings (public)
public static FoodServing PumpkinSeeds_30_Grams => 
    PumpkinSeeds.WithServing(30, ServingUnits.Gram);

public static FoodServing PumpkinSeeds_1_Scoop => 
    PumpkinSeeds.WithServing(1, ServingUnits.Scoop);
```

## Implementation Steps

### Phase 1: Create New Structure
1. Create `Executable/BaseNutrition.cs`
2. Create new `Executable/Food.cs` with base food type logic
3. Rename existing `Executable/Food.cs` to `Executable/FoodServing.cs`
4. Update `FoodServing.cs` to include multiplication operator from `Helping`
5. Delete `Executable/Helping.cs`

### Phase 2: Update Food Definitions
6. Refactor `Executable/Data/Foods.cs`:
   - Define each food once as a private `Food` with equivalences
   - Create public `FoodServing` properties using `WithServing()`
   - Maintain same public API for backward compatibility

### Phase 3: Update References
7. Update `Executable/Meal.cs`:
   - Change `IEnumerable<Helping>` to `IEnumerable<FoodServing>`
   - Update `Helpings` property logic

8. Update `Executable/FoodGrouping.cs`:
   - Change `StaticHelpings` to `StaticServings`
   - Update type from `Helping[]` to `FoodServing[]`

9. Update `Executable/Data/FoodGroupings.cs`:
   - Replace `new Helping(food, servings)` with `food * servings`

10. Update all training week files in `Executable/Data/TrainingWeeks/`:
    - Replace `Helping` references with `FoodServing`

11. Update test files:
    - Fix any broken tests due to class changes

### Phase 4: Testing
12. Run all tests to ensure:
    - Nutritional calculations remain correct
    - Meal planning still works
    - Unit conversions function properly

## Benefits

1. **Clarity**: Clear separation between a food and a serving of that food
2. **No Redundancy**: Each food is defined exactly once
3. **Explicit Equivalences**: Serving equivalences (30g = 0.25 cups) are visible in code
4. **Simpler**: Removes unnecessary `Helping` wrapper class
5. **Maintainable**: Adding new serving sizes is trivial
6. **Type Safety**: Maintains compile-time type checking

## Migration Notes

### Breaking Changes
- `Helping` class no longer exists
- `Food` class renamed to `FoodServing`
- New `Food` class has different structure

### Backward Compatibility
- All public static properties in `Foods.cs` maintain same names
- Return type changes from `Food` to `FoodServing` but interface remains similar
- Existing meal calculations continue to work

## Example Usage

```csharp
// Getting a specific serving
var serving = Foods.PumpkinSeeds_30_Grams;  // Returns FoodServing

// Creating custom serving sizes
var customServing = PumpkinSeeds.WithServing(45, ServingUnits.Gram);

// Scaling servings
var doubleServing = Foods.PumpkinSeeds_1_Scoop * 2;

// In meal definitions
var meal = new Meal("Breakfast", macros, new FoodGrouping(
    "Oatmeal Bowl",
    [Foods.Oats_1_Scoop * 1.5M, Foods.BlueBerries_1_Cup * 0.5M],
    // ...
));
```

## Files Affected

- **New Files:**
  - `Executable/BaseNutrition.cs`
  - `Executable/Food.cs` (new version)

- **Modified Files:**
  - `Executable/Food.cs` → `Executable/FoodServing.cs`
  - `Executable/Data/Foods.cs`
  - `Executable/Meal.cs`
  - `Executable/FoodGrouping.cs`
  - `Executable/Data/FoodGroupings.cs`
  - `Executable/Data/TrainingWeeks/*.cs`
  - `Executable/Data/WeeklyMealsPrepPlans.cs`
  - Test files as needed

- **Deleted Files:**
  - `Executable/Helping.cs`

## Success Criteria

- All tests pass
- No runtime errors in meal planning
- Nutritional calculations remain accurate
- Code is more maintainable and easier to understand