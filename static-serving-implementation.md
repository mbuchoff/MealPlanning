# Static Serving Components Implementation

## Overview

This implementation adds the ability to create composite foods with **static serving components** - ingredients that maintain their fixed amounts regardless of how much the composite food is scaled.

## Background

Previously, when a `CompositeFoodServing` was multiplied (e.g., `composite * 2`), ALL components would scale proportionally. This works well for most ingredients but not for seasonings, spices, or other additions that should remain constant.

### Example Problem

```csharp
// Before: All components scale together
var meal = CompositeFoodServing.FromComponents("Seasoned Chicken",
    [proteinServing, saltServing]);

var doubled = meal * 2;
// Result: 200g protein + 2 tsp salt (salt doubled too!)
```

### Solution

```csharp
// After: Static components stay constant
var meal = CompositeFoodServing.FromComponentsWithStatic("Seasoned Chicken",
    scalableComponents: [proteinServing],
    staticComponents: [saltServing]);

var doubled = meal * 2;
// Result: 200g protein + 1 tsp salt (salt stays same!)
```

## Implementation Details

### Core Classes

#### 1. StaticFoodServing

A wrapper around `FoodServing` that prevents scaling:

```csharp
public record StaticFoodServing : FoodServing
{
    public FoodServing OriginalServing { get; }

    // Key feature: Multiplication returns unchanged instance
    public static StaticFoodServing operator *(StaticFoodServing staticServing, decimal multiplier) =>
        staticServing; // No scaling!
}
```

#### 2. Enhanced CompositeFoodServing

Updated to handle mixed component types:

- **FromComponentsWithStatic()** - New factory method for mixed components
- **Type-aware scaling** - Checks component types at runtime
- **Preserves behavior** - Existing functionality unchanged

### Key Design Decisions

#### Option 1: Wrapper Class ✅ (Chosen)
- Clean separation of concerns
- Preserves existing API
- Easy to understand and use
- Minimal code changes required

#### Option 2: Boolean Flag ❌ (Rejected)
- Would require modifying existing `FoodServing` class
- More invasive changes
- Less clear intent

#### Option 3: Separate Collections ❌ (Rejected)
- Most complex to implement
- Would break existing API patterns
- Harder to use

## Usage Examples

### Basic Static Components

```csharp
// Create individual servings
var chicken = new FoodServing("chicken breast",
    new NutritionalInformation(100, ServingUnits.Gram, 165, 31, 3.6M, 0, 0));
var salt = new FoodServing("salt",
    new NutritionalInformation(1, ServingUnits.Tablespoon, 0, 0, 0, 0, 0));

// Create composite with mixed scaling behavior
var seasonedChicken = CompositeFoodServing.FromComponentsWithStatic(
    "Seasoned Chicken",
    scalableComponents: [chicken],
    staticComponents: [salt]);

// Scale the meal
var doubledMeal = seasonedChicken * 2;
// Output: "200 grams chicken breast\n1.0 tbsp salt"
```

### Manual Static Wrapping

```csharp
// Manually wrap components as static
var staticSalt = new StaticFoodServing(salt);
var staticSpice = new StaticFoodServing(spice);

var composite = CompositeFoodServing.FromComponents("Spiced Meal",
    [protein, vegetable, staticSalt, staticSpice]);
```

### Complex Nesting

Static components work correctly even in nested composites:

```csharp
var innerComposite = CompositeFoodServing.FromComponentsWithStatic(
    "Seasoned Protein",
    scalableComponents: [protein],
    staticComponents: [salt]);

var outerComposite = CompositeFoodServing.FromComponentsWithStatic(
    "Full Meal",
    scalableComponents: [innerComposite, vegetables],
    staticComponents: [pepper]);

var scaled = outerComposite * 3;
// Protein and vegetables scale 3x, salt and pepper stay same
```

## Technical Implementation

### Operator Overloading Challenge

The main technical challenge was C#'s static operator dispatch. Since operators don't support virtual dispatch, the compiler chooses which operator to call based on compile-time types.

**Problem:**
```csharp
// Even if 'component' is actually a StaticFoodServing,
// this calls FoodServing.operator * because 'component' is typed as FoodServing
FoodServing component = new StaticFoodServing(salt);
var scaled = component * 2; // Calls wrong operator!
```

**Solution:**
Runtime type checking in `CompositeFoodServing`:

```csharp
foreach (var component in Components)
{
    if (component is StaticFoodServing staticComponent)
    {
        yield return staticComponent; // Don't scale
    }
    else
    {
        yield return component * scale; // Scale normally
    }
}
```

### Methods Updated

All display methods in `CompositeFoodServing` needed updates:

1. **ToString()** - String representation with proper scaling
2. **ToOutputLines()** - Line-by-line output for display
3. **GetComponentsForDisplay()** - Component enumeration for UI

## Test-Driven Development

The implementation followed strict TDD:

1. **Write failing tests** - 10 comprehensive test cases
2. **Confirm failures** - All tests failed as expected initially
3. **Implement incrementally** - Build features to make tests pass
4. **Verify completion** - All tests pass, no regressions

### Test Coverage

- ✅ Basic static component behavior
- ✅ Mixed scalable/static components
- ✅ String output formatting
- ✅ Nutritional calculation accuracy
- ✅ Factory method functionality
- ✅ Complex nesting scenarios
- ✅ Edge cases (water, empty components)
- ✅ Integration with existing features

## Benefits

### For Recipe Development
- Seasonings stay proportional to recipe base
- Prevents over-seasoning when scaling recipes
- More realistic cooking behavior

### For Nutrition Tracking
- Accurate nutritional information at any scale
- Separates variable from fixed components
- Better meal planning flexibility

### For Code Quality
- Backward compatible - no breaking changes
- Clear intent through type system
- Comprehensive test coverage

## Usage Guidelines

### When to Use Static Components

**Good candidates:**
- Salt, spices, seasonings
- Garnishes and toppings
- Cooking oils (small amounts)
- Supplements or vitamins
- Water for cooking (base amount)

**Poor candidates:**
- Main protein sources
- Bulk vegetables or grains
- Primary macronutrient sources
- Components that should scale linearly

### Best Practices

1. **Use the factory method** when possible:
   ```csharp
   CompositeFoodServing.FromComponentsWithStatic(name, scalable, static)
   ```

2. **Group similar components** by scaling behavior

3. **Test scaling behavior** with different multipliers

4. **Document static choices** in food names when helpful

## Future Enhancements

Potential future improvements:

1. **Percentage-based scaling** - Components that scale by different rates
2. **Conditional scaling** - Components that scale only within certain ranges
3. **Visual indicators** - UI markers for static vs scalable components
4. **Recipe templates** - Predefined patterns for common food types

## Conclusion

The static serving implementation successfully addresses the real-world need for non-scaling food components while maintaining backward compatibility and following TDD principles. It provides a clean, intuitive API that makes food composition more accurate and realistic.