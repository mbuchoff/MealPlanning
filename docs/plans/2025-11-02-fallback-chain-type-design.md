# FallbackChain Type Design

**Date:** 2025-11-02
**Status:** Approved

## Problem Statement

The `FoodGroupings` class contains properties that return `FoodGrouping[]` to represent fallback chains, but the type doesn't indicate whether a property is a simple grouping or a fallback chain. For example, `FoodGroupings.Ezekiel` returns a `FoodGrouping[]` with two fallback options, but this is not obvious from the property name or type signature.

## Goals

1. Make fallback chains explicit in the type system
2. Distinguish between single `FoodGrouping` and `FallbackChain` at compile time
3. Improve code readability and maintainability
4. Prevent misuse (e.g., treating a single grouping as a chain or vice versa)

## Design Overview

Introduce a new `FallbackChain` record type that wraps an ordered list of `FoodGrouping` options. This provides type-level distinction between simple groupings and fallback chains.

## Core Type: FallbackChain

```csharp
public record FallbackChain
{
    private readonly FoodGrouping[] _groupings;

    public FallbackChain(params FoodGrouping[] groupings)
    {
        if (groupings.Length < 2)
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
```

### Key Properties

- **Primary**: The first (preferred) food grouping
- **Fallbacks**: Subsequent options tried if Primary fails
- **All**: Complete array for iteration during calculation
- **Count**: Number of groupings in the chain

### Validation

Constructor enforces minimum 2 groupings, preventing meaningless single-item "fallback chains".

## Integration with Meal Class

### Constructor Changes

```csharp
public record Meal
{
    // Existing: single FoodGrouping
    public Meal(string name, Macros macros, FoodGrouping grouping) { ... }

    // NEW: FallbackChain
    public Meal(string name, Macros macros, FallbackChain fallbackChain) { ... }
}
```

### Removed API

**Remove:** `Meal.WithFallbacks` static method

**Reason:** The `FallbackChain` constructor with `params` provides the same convenience:
- Old: `Meal.WithFallbacks("name", macros, g1, g2, g3)`
- New: `new Meal("name", macros, new FallbackChain(g1, g2, g3))`

When using predefined chains from `FoodGroupings`, the usage is even cleaner:
- `new Meal("name", macros, FoodGroupings.Ezekiel)`

## Changes to FoodGroupings Class

### Before
```csharp
public static FoodGrouping[] Ezekial { get; } = [.. new[] { ... }.Select(...)];
```

### After
```csharp
public static FallbackChain Ezekiel { get; } = new(
    new FoodGrouping("Ezekiel", [...], Foods.Edamame_1_Scoop, ...),
    new FoodGrouping("Ezekiel", [...], Foods.ProteinToCarbConversion, ...)
);
```

### Property Type Pattern

- Simple groupings: Return `FoodGrouping` (unchanged)
- Fallback chains: Return `FallbackChain` (changed from `FoodGrouping[]`)

This makes the distinction immediately visible in the API.

## Internal Meal Representation

```csharp
public record Meal
{
    private readonly FoodGrouping? _singleGrouping;
    private readonly FallbackChain? _fallbackChain;

    public Meal(string name, Macros macros, FoodGrouping grouping)
    {
        _singleGrouping = grouping;
        _fallbackChain = null;
        // ...
    }

    public Meal(string name, Macros macros, FallbackChain fallbackChain)
    {
        _singleGrouping = null;
        _fallbackChain = fallbackChain;
        // ...
    }
}
```

Store either a single grouping OR a fallback chain (mutually exclusive).

## Calculation Logic

```csharp
private FoodGrouping? FindWorkingGrouping()
{
    if (_singleGrouping != null)
        return TryGrouping(_singleGrouping) ? _singleGrouping : null;

    // Iterate through fallback chain
    foreach (var grouping in _fallbackChain!.All)
    {
        if (TryGrouping(grouping))
            return grouping;
    }
    return null;
}
```

Use `FallbackChain.All` for iteration. The selected grouping becomes the actual grouping used for the meal.

## Migration Impact

### Breaking Changes
- `FoodGroupings` properties returning `FoodGrouping[]` change to `FallbackChain`
- `Meal.WithFallbacks` method removed
- Call sites in `TrainingWeeks` must update to use new constructors

### Files Requiring Updates
- `Executable/FoodGrouping.cs` - Add new `FallbackChain` record
- `Executable/Meal.cs` - Update constructors, remove `WithFallbacks`, update calculation logic
- `Executable/Data/FoodGroupings.cs` - Change array properties to `FallbackChain`
- `Executable/Data/TrainingWeeks/*.cs` - Update meal creation calls
- Tests - Update any tests creating meals with fallbacks

### Non-Breaking
- Simple `FoodGrouping` properties remain unchanged
- Existing single-grouping meal creation unchanged

## Benefits

1. **Type Safety**: Compiler distinguishes single groupings from fallback chains
2. **Clarity**: `FoodGrouping` vs `FallbackChain` is immediately obvious
3. **Validation**: Constructor prevents invalid single-item chains
4. **Intent**: Properties like `Primary` and `Fallbacks` make purpose explicit
5. **Discoverability**: Easier to find all fallback chains in the codebase

## Testing Considerations

- Test `FallbackChain` constructor validation (rejects < 2 items)
- Test `Primary`, `Fallbacks`, `All` properties
- Test meal calculation with both single groupings and fallback chains
- Verify existing meal calculation tests still pass
- Add tests for edge cases (all fallbacks fail, etc.)
