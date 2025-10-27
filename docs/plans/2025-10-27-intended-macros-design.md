# Add INTENDED Macros to Todoist Output

**Date:** 2025-10-27

## Problem

When outputting meal macros to Todoist, conversion foods are intentionally excluded from the totals. This shows the ACTUAL macros from real food consumed, but doesn't show the INTENDED macros that include macro adjustments from conversion foods (e.g., Protein to Carb Conversion).

Users need to see both values to understand:
- **ACTUAL:** What macros they're actually consuming from real food
- **INTENDED:** What the original meal plan specified including conversions

## Solution

Extend `TodoistServiceHelper.GenerateNutritionalComment` to calculate and display both ACTUAL and INTENDED macro totals.

### Output Format

```
ACTUAL: {macro totals excluding conversion foods}

INTENDED: {macro totals including all foods}

{Individual Serving 1}
{nutrients}

{Individual Serving 2}
{nutrients}
...
```

## Implementation

### Changes to TodoistServiceHelper.cs

Modify `GenerateNutritionalComment` method:

1. **Calculate ACTUAL total** (existing logic, add label):
   ```csharp
   var nonConversionServings = servings.Where(s => !s.IsConversion).ToList();
   var actualTotal = nonConversionServings
       .Select(s => s.NutritionalInformation)
       .Sum(1, ServingUnits.Meal);
   ```

2. **Calculate INTENDED total** (new):
   ```csharp
   var intendedTotal = servings
       .Select(s => s.NutritionalInformation)
       .Sum(1, ServingUnits.Meal);
   ```

3. **Build output string**:
   ```csharp
   var comment = string.Join("\n\n",
       new[] {
           $"ACTUAL:\n{actualTotal.ToNutrientsString()}",
           $"INTENDED:\n{intendedTotal.ToNutrientsString()}"
       }.Concat(
           nonConversionServings.Select(s =>
               $"{s.Name}\n{s.NutritionalInformation.ToNutrientsString()}")));
   ```

### Changes to TodoistServiceTests.cs

1. **Add new test:** `GenerateNutritionalComment_Should_Include_Intended_Macros_With_Conversion_Foods`
   - Verify both ACTUAL and INTENDED sections present
   - Verify ACTUAL excludes conversion foods
   - Verify INTENDED includes conversion foods
   - Verify labels are correct
   - Verify ordering (ACTUAL, INTENDED, then individuals)

2. **Update existing test:** `GenerateNutritionalComment_Should_Exclude_Conversion_Servings_From_Macro_Totals`
   - Update assertions to look for "ACTUAL:" label in first section

## Edge Cases

1. **No conversion foods:** ACTUAL and INTENDED will be identical (acceptable)
2. **All conversion foods:** ACTUAL shows zeros, INTENDED shows totals
3. **Empty servings:** Same behavior as current implementation

## Testing

All existing tests must continue passing with the label addition. The new test verifies the intended functionality works correctly with conversion foods.

## Files Modified

- `Executable/Todoist/TodoistServiceHelper.cs`
- `Test/TodoistServiceTests.cs`
