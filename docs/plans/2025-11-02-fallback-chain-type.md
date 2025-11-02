# FallbackChain Type Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Introduce a `FallbackChain` record type to make fallback chains explicit in the type system, replacing `FoodGrouping[]` arrays.

**Architecture:** Create immutable `FallbackChain` record with validation (min 2 items) and helper properties (`Primary`, `Fallbacks`, `All`). Update `Meal` class to accept either single `FoodGrouping` or `FallbackChain`. Refactor `FoodGroupings` properties from arrays to `FallbackChain`. Remove `Meal.WithFallbacks` method as it becomes redundant.

**Tech Stack:** .NET 9.0, C# records, xUnit

---

## Task 1: Create FallbackChain Record with Tests

### Step 1: Write failing test for FallbackChain constructor validation

**Files:**
- Create: `Test/FallbackChainTests.cs`

```csharp
// ABOUTME: Tests for FallbackChain record type to verify construction, validation, and properties
using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class FallbackChainTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Less_Than_Two_Groupings()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood = new FoodServing("Carb",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));

        var singleGrouping = new FoodGrouping("Single", [], pFood, fFood, cFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new FallbackChain(singleGrouping));
        Assert.Contains("at least 2", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
```

### Step 2: Run test to verify it fails

Run: `dotnet test --filter "FullyQualifiedName~FallbackChainTests.Constructor_Should_Throw_When_Less_Than_Two_Groupings"`

Expected: FAIL with "The type or namespace name 'FallbackChain' could not be found"

### Step 3: Write FallbackChain record

**Files:**
- Create: `Executable/FallbackChain.cs`

```csharp
// ABOUTME: Represents an ordered chain of FoodGrouping options where each subsequent option
// is tried if the previous one fails to produce valid servings
namespace SystemOfEquations;

public record FallbackChain
{
    private readonly FoodGrouping[] _groupings;

    public FallbackChain(params FoodGrouping[] groupings)
    {
        if (groupings == null || groupings.Length < 2)
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

### Step 4: Run test to verify it passes

Run: `dotnet test --filter "FullyQualifiedName~FallbackChainTests.Constructor_Should_Throw_When_Less_Than_Two_Groupings"`

Expected: PASS

### Step 5: Add tests for FallbackChain properties

Add to `Test/FallbackChainTests.cs`:

```csharp
[Fact]
public void Primary_Should_Return_First_Grouping()
{
    // Arrange
    var pFood = new FoodServing("Protein",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
    var fFood = new FoodServing("Fat",
        new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
    var cFood1 = new FoodServing("Carb1",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
    var cFood2 = new FoodServing("Carb2",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

    var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    var chain = new FallbackChain(grouping1, grouping2);

    // Act & Assert
    Assert.Same(grouping1, chain.Primary);
}

[Fact]
public void Fallbacks_Should_Return_All_Except_First()
{
    // Arrange
    var pFood = new FoodServing("Protein",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
    var fFood = new FoodServing("Fat",
        new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
    var cFood1 = new FoodServing("Carb1",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
    var cFood2 = new FoodServing("Carb2",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));
    var cFood3 = new FoodServing("Carb3",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 80, P: 1, F: 0, CTotal: 20, CFiber: 4));

    var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping3 = new FoodGrouping("Grouping3", [], pFood, fFood, cFood3,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    var chain = new FallbackChain(grouping1, grouping2, grouping3);

    // Act & Assert
    Assert.Equal(2, chain.Fallbacks.Length);
    Assert.Same(grouping2, chain.Fallbacks[0]);
    Assert.Same(grouping3, chain.Fallbacks[1]);
}

[Fact]
public void All_Should_Return_Complete_Array()
{
    // Arrange
    var pFood = new FoodServing("Protein",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
    var fFood = new FoodServing("Fat",
        new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
    var cFood1 = new FoodServing("Carb1",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
    var cFood2 = new FoodServing("Carb2",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

    var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    var chain = new FallbackChain(grouping1, grouping2);

    // Act & Assert
    Assert.Equal(2, chain.All.Length);
    Assert.Same(grouping1, chain.All[0]);
    Assert.Same(grouping2, chain.All[1]);
}

[Fact]
public void Count_Should_Return_Number_Of_Groupings()
{
    // Arrange
    var pFood = new FoodServing("Protein",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
    var fFood = new FoodServing("Fat",
        new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
    var cFood1 = new FoodServing("Carb1",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
    var cFood2 = new FoodServing("Carb2",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

    var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    var chain = new FallbackChain(grouping1, grouping2);

    // Act & Assert
    Assert.Equal(2, chain.Count);
}
```

### Step 6: Run tests to verify they pass

Run: `dotnet test --filter "FullyQualifiedName~FallbackChainTests"`

Expected: PASS (5 tests total)

### Step 7: Commit

```bash
git add Executable/FallbackChain.cs Test/FallbackChainTests.cs
git commit -m "feat: add FallbackChain record type with validation and properties"
```

---

## Task 2: Add Meal Constructor Accepting FallbackChain

### Step 1: Write failing test for Meal with FallbackChain

Add to `Test/MealFallbackTests.cs`:

```csharp
[Fact]
public void Meal_Should_Accept_FallbackChain_In_Constructor()
{
    // Arrange
    var pFood = new FoodServing("Protein",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
    var fFood = new FoodServing("Fat",
        new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
    var cFood1 = new FoodServing("Carb1",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
    var cFood2 = new FoodServing("Carb2",
        new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

    var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
    var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
        FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

    var fallbackChain = new FallbackChain(grouping1, grouping2);

    // Act
    var meal = new Meal("Test Meal", new Macros(P: 30, F: 15, C: 50), fallbackChain);

    // Assert
    Assert.NotNull(meal);
    Assert.Equal("Test Meal", meal.Name);
}
```

### Step 2: Run test to verify it fails

Run: `dotnet test --filter "FullyQualifiedName~MealFallbackTests.Meal_Should_Accept_FallbackChain_In_Constructor"`

Expected: FAIL with "no overload for method 'Meal' takes 3 arguments"

### Step 3: Add constructor to Meal class

In `Executable/Meal.cs`, add new constructor after the existing single-FoodGrouping constructor (around line 15):

```csharp
public Meal(string name, Macros macros, FallbackChain fallbackChain)
{
    Name = name;
    Macros = macros;
    _foodGroupings = fallbackChain?.All ?? throw new ArgumentNullException(nameof(fallbackChain));
    if (_foodGroupings.Length == 0)
        throw new ArgumentException("FallbackChain must contain at least one FoodGrouping", nameof(fallbackChain));
}
```

### Step 4: Run test to verify it passes

Run: `dotnet test --filter "FullyQualifiedName~MealFallbackTests.Meal_Should_Accept_FallbackChain_In_Constructor"`

Expected: PASS

### Step 5: Commit

```bash
git add Executable/Meal.cs Test/MealFallbackTests.cs
git commit -m "feat: add Meal constructor accepting FallbackChain"
```

---

## Task 3: Update FoodGroupings to Use FallbackChain

### Step 1: Update Ezekiel property in FoodGroupings

In `Executable/Data/FoodGroupings.cs`, change the `Ezekial` property (line 15):

**Before:**
```csharp
public static FoodGrouping[] Ezekial { get; } = [.. new[] { Foods.Edamame_1_Scoop, Foods.ProteinToCarbConversion }.Select(pFood =>
    new FoodGrouping(
        "Ezekial",
        [Foods.AlmondMilk_1_Scoop * 2],
        Foods.PumpkinSeeds_1_Scoop,
        pFood,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded))];
```

**After:**
```csharp
public static FallbackChain Ezekiel { get; } = new(
    new FoodGrouping(
        "Ezekiel",
        [Foods.AlmondMilk_1_Scoop * 2],
        Foods.PumpkinSeeds_1_Scoop,
        Foods.Edamame_1_Scoop,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded),
    new FoodGrouping(
        "Ezekiel",
        [Foods.AlmondMilk_1_Scoop * 2],
        Foods.PumpkinSeeds_1_Scoop,
        Foods.ProteinToCarbConversion,
        Foods.Ezeliel_Cereal_Low_Sodium_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded));
```

### Step 2: Update methods returning FoodGrouping[] in FoodGroupings

Update `Oatmeal` method (line 39):

**Before:**
```csharp
public static FoodGrouping Oatmeal(bool withEdamame = true) => new(
    "Oatmeal and edamame",
    withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
    Foods.AlmondButter_1_Tbsp,
    Foods.Oats_1_Scoop,
    PreparationMethodEnum.PrepareAsNeeded);
```

**After:**
```csharp
public static FallbackChain Oatmeal(bool withEdamame = true) => new(
    new FoodGrouping(
        "Oatmeal and edamame",
        [],
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
        Foods.AlmondButter_1_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded),
    new FoodGrouping(
        "Oatmeal and edamame",
        [],
        withEdamame ? Foods.ProteinToCarbConversion : Foods.Edamame_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded));
```

Update `EnglishMuffinsAndPasta` method (line 46):

**Before:**
```csharp
public static FoodGrouping EnglishMuffinsAndPasta(int englishMuffins, bool withEdamame = true) => new(
    "English muffins and pasta",
    englishMuffins == 0 ? [] : [Foods.Ezekiel_English_Muffin * englishMuffins],
    withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
    Foods.OliveOil_1_Tbsp,
    Foods.Whole_Grain_Pasta_56_Grams,
    PreparationMethodEnum.PrepareAsNeeded);
```

**After:**
```csharp
public static FallbackChain EnglishMuffinsAndPasta(int englishMuffins, bool withEdamame = true) => new(
    new FoodGrouping(
        "English muffins and pasta",
        englishMuffins == 0 ? [] : [Foods.Ezekiel_English_Muffin * englishMuffins],
        withEdamame ? Foods.Edamame_1_Scoop : Foods.ProteinToCarbConversion,
        Foods.OliveOil_1_Tbsp,
        Foods.Whole_Grain_Pasta_56_Grams,
        PreparationMethodEnum.PrepareAsNeeded),
    new FoodGrouping(
        "English muffins and pasta",
        englishMuffins == 0 ? [] : [Foods.Ezekiel_English_Muffin * englishMuffins],
        withEdamame ? Foods.ProteinToCarbConversion : Foods.Edamame_1_Scoop,
        Foods.OliveOil_1_Tbsp,
        Foods.Whole_Grain_Pasta_56_Grams,
        PreparationMethodEnum.PrepareAsNeeded));
```

### Step 3: Build to check for compilation errors

Run: `dotnet build`

Expected: Compilation errors in TrainingWeek files (we'll fix those next)

### Step 4: Commit FoodGroupings changes

```bash
git add Executable/Data/FoodGroupings.cs
git commit -m "refactor: change FoodGroupings properties from arrays to FallbackChain"
```

---

## Task 4: Update TrainingWeek Files to Use New Constructors

### Step 1: Update MuscleGain2.cs

In `Executable/Data/TrainingWeeks/MuscleGain2.cs`:

**Change line 11** from:
```csharp
Meal.WithFallbacks("Waking", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60), WakingBlueberryOatmealShakeFoodGroupings),
```

To:
```csharp
new Meal("Waking", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 10, C: 60), WakingBlueberryOatmealShakeFoodGroupings),
```

**Change line 12** from:
```csharp
Meal.WithFallbacks("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Ezekial),
```

To:
```csharp
new Meal("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 20, C: 60), FoodGroupings.Ezekiel),
```

**Change line 15** from:
```csharp
Meal.WithFallbacks("Bedtime", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0), BedtimeProteinShakeFoodGroupings)
```

To:
```csharp
new Meal("Bedtime", new Macros(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_NONWORKOUT_DAY, F: 25, C: 0), BedtimeProteinShakeFoodGroupings)
```

**Change line 23** from:
```csharp
Meal.WithFallbacks("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 100), FoodGroupings.Ezekial),
```

To:
```csharp
new Meal("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 100), FoodGroupings.Ezekiel),
```

**Change line 30** from:
```csharp
Meal.WithFallbacks("1-3 hours before workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80), Oatmeal),
```

To:
```csharp
new Meal("1-3 hours before workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 80), Oatmeal),
```

**Change line 32** from:
```csharp
Meal.WithFallbacks("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), WheatBerriesAndRice),
```

To:
```csharp
new Meal("40 minutes after workout", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 10, C: 120), WheatBerriesAndRice),
```

**Change line 33** from:
```csharp
Meal.WithFallbacks("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100), WheatBerriesAndRice),
```

To:
```csharp
new Meal("2-4 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 100), WheatBerriesAndRice),
```

**Change line 34** from:
```csharp
Meal.WithFallbacks("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Ezekial),
```

To:
```csharp
new Meal("3-5 hours after last meal", new(P: MUSCLE_GAIN_PROTEIN_PER_MEAL_ON_WORKOUT_DAY, F: 20, C: 50), FoodGroupings.Ezekiel),
```

**Change private properties** (lines 41-80) from `FoodGrouping[]` to `FallbackChain`:

Change line 41:
```csharp
private static readonly FallbackChain Oatmeal =
```

Change line 51:
```csharp
private static FallbackChain WheatBerriesAndRice { get; } =
```

Change line 60:
```csharp
private static readonly FallbackChain WakingBlueberryOatmealShakeFoodGroupings =
```

Change line 73:
```csharp
private static readonly FallbackChain BedtimeProteinShakeFoodGroupings =
```

**Update property initialization** - wrap array expressions with `new FallbackChain(...)`:

Line 41-49:
```csharp
private static readonly FallbackChain Oatmeal = new(
    new[] { Foods.Edamame_1_Scoop, Foods.ProteinToFatConversion }
        .Select(pFood => new FoodGrouping(
        "blueberries and oatmeal",
        [Foods.Ezekiel_English_Muffin, Foods.BlueBerries_1_Scoop * 4, Foods.Creatine_1_Scoop],
        pFood,
        Foods.ChiaSeeds_2_5_Tbsp,
        Foods.Oats_1_Scoop,
    PreparationMethodEnum.PrepareAsNeeded)).ToArray());
```

Line 51-58:
```csharp
private static FallbackChain WheatBerriesAndRice { get; } = new(
    new[] { Foods.WheatBerries_45_Grams, Foods.ProteinToCarbConversion }.Select(pFood => new FoodGrouping(
        "rice",
        [Foods.Ezekiel_English_Muffin * 1],
        pFood,
        Foods.PumpkinSeeds_30_Grams,
        Foods.BrownRice_45_Grams,
        PreparationMethodEnum.PrepareInAdvance)).ToArray());
```

Line 60-71:
```csharp
private static readonly FallbackChain WakingBlueberryOatmealShakeFoodGroupings = new(
    [.. new[] { Foods.AlmondButter_1_Tbsp, Foods.FatToCarbConversion }.Select(fFood =>
    new FoodGrouping("Blueberry oatmeal shake",
        [
            Foods.BlueBerries_1_Scoop * 3,
            Foods.AlmondMilk_2_Cup,
            Foods.Creatine_1_Scoop,
        ],
        Foods.PeaProtein_1_Scoop,
        fFood,
        Foods.Oats_1_Scoop,
        PreparationMethodEnum.PrepareAsNeeded))]);
```

Line 73-80:
```csharp
private static readonly FallbackChain BedtimeProteinShakeFoodGroupings = new(
    [.. new[] { Foods.BlueBerries_1_Scoop, Foods.FatToCarbConversion }.Select(cFood =>
    new FoodGrouping("Protein shake",
        [Foods.AlmondMilk_2_Cup],
        Foods.PeaProtein_1_Scoop,
        Foods.AlmondButter_1_Tbsp,
        cFood,
        PreparationMethodEnum.PrepareAsNeeded))]);
```

### Step 2: Update MuscleGain3.cs

In `Executable/Data/TrainingWeeks/MuscleGain3.cs`, find all uses of `Meal.WithFallbacks` and replace with `new Meal`. Find all uses of `FoodGroupings.Ezekial` and replace with `FoodGroupings.Ezekiel`. Find all private `FoodGrouping[]` properties and change to `FallbackChain`, wrapping initializers with `new FallbackChain(...)`.

### Step 3: Update Base/BaseWorkingOutInMorning.cs

Apply same changes as MuscleGain files.

### Step 4: Update Base/BaseWorkingOutInEvening.cs

Apply same changes as MuscleGain files.

### Step 5: Update MuscleGain1.cs

Apply same changes, if it uses any fallback patterns.

### Step 6: Update FatLoss1.cs

Apply same changes, if it uses any fallback patterns.

### Step 7: Build to verify no compilation errors

Run: `dotnet build`

Expected: Build succeeds with 0 warnings, 0 errors

### Step 8: Commit TrainingWeek changes

```bash
git add Executable/Data/TrainingWeeks/
git commit -m "refactor: update TrainingWeek files to use FallbackChain"
```

---

## Task 5: Remove Meal.WithFallbacks Method

### Step 1: Remove WithFallbacks method from Meal.cs

In `Executable/Meal.cs`, delete lines 17-20:

```csharp
public static Meal WithFallbacks(string name, Macros macros, params FoodGrouping[] foodGroupings)
{
    return new Meal(name, macros, foodGroupings);
}
```

And delete the private constructor (lines 22-29):

```csharp
private Meal(string name, Macros macros, params FoodGrouping[] foodGroupings)
{
    Name = name;
    Macros = macros;
    _foodGroupings = foodGroupings ?? throw new ArgumentNullException(nameof(foodGroupings));
    if (foodGroupings.Length == 0)
        throw new ArgumentException("At least one FoodGrouping must be provided", nameof(foodGroupings));
}
```

### Step 2: Build to verify no compilation errors

Run: `dotnet build`

Expected: Build succeeds

### Step 3: Commit removal

```bash
git add Executable/Meal.cs
git commit -m "refactor: remove Meal.WithFallbacks method (now redundant)"
```

---

## Task 6: Update Meal Internal Logic for FallbackChain

### Step 1: Update Meal properties to work with new representation

In `Executable/Meal.cs`, update the `FoodGroupings` property (line 33) to handle FallbackChain:

**Before:**
```csharp
public FoodGrouping[] FoodGroupings => _foodGroupings;
```

**After:**
```csharp
public FoodGrouping[] FoodGroupings => _foodGroupings;
```

(No change needed - property still returns array which both constructors populate)

### Step 2: Update CloneWithTweakedMacros to use constructor directly

In `Executable/Meal.cs`, update `CloneWithTweakedMacros` method (line 157):

**Before:**
```csharp
return WithFallbacks(Name, tweakedMacros, FoodGroupings);
```

**After:**
```csharp
if (_foodGroupings.Length == 1)
{
    return new Meal(Name, tweakedMacros, _foodGroupings[0]);
}
else
{
    return new Meal(Name, tweakedMacros, new FallbackChain(_foodGroupings));
}
```

### Step 3: Update SumWithSameFoodGrouping to use constructor directly

In `Executable/Meal.cs`, update line 189 in `SumWithSameFoodGrouping`:

**Before:**
```csharp
var meal = Meal.WithFallbacks(scaledFoodGroupings[0].Name, totalMacros, scaledFoodGroupings);
```

**After:**
```csharp
var meal = scaledFoodGroupings.Length == 1
    ? new Meal(scaledFoodGroupings[0].Name, totalMacros, scaledFoodGroupings[0])
    : new Meal(scaledFoodGroupings[0].Name, totalMacros, new FallbackChain(scaledFoodGroupings));
```

### Step 4: Run all tests

Run: `dotnet test`

Expected: All tests pass

### Step 5: Commit internal logic updates

```bash
git add Executable/Meal.cs
git commit -m "refactor: update Meal internal logic to use FallbackChain constructors"
```

---

## Task 7: Update Existing Tests

### Step 1: Update MealFallbackTests to use new constructors

In `Test/MealFallbackTests.cs`, update test at line 29:

**Before:**
```csharp
var originalMeal = Meal.WithFallbacks("Test Meal", new Macros(P: 30, F: 15, C: 50),
    foodGrouping1, foodGrouping2);
```

**After:**
```csharp
var originalMeal = new Meal("Test Meal", new Macros(P: 30, F: 15, C: 50),
    new FallbackChain(foodGrouping1, foodGrouping2));
```

Update test at line 65:

**Before:**
```csharp
Meal.WithFallbacks("Meal 1", new Macros(P: 30, F: 15, C: 50), foodGrouping1, foodGrouping2),
Meal.WithFallbacks("Meal 2", new Macros(P: 35, F: 20, C: 60), foodGrouping1, foodGrouping2),
```

**After:**
```csharp
new Meal("Meal 1", new Macros(P: 30, F: 15, C: 50), new FallbackChain(foodGrouping1, foodGrouping2)),
new Meal("Meal 2", new Macros(P: 35, F: 20, C: 60), new FallbackChain(foodGrouping1, foodGrouping2)),
```

### Step 2: Check if other test files use Meal.WithFallbacks

Search for `Meal.WithFallbacks` in test files:

Run: `grep -r "Meal.WithFallbacks" Test/`

Update any found occurrences to use `new Meal(..., new FallbackChain(...))`.

### Step 3: Run all tests

Run: `dotnet test`

Expected: All 81+ tests pass

### Step 4: Commit test updates

```bash
git add Test/
git commit -m "test: update tests to use new Meal and FallbackChain constructors"
```

---

## Task 8: Final Verification and Documentation

### Step 1: Run full test suite

Run: `dotnet test`

Expected: All tests pass

### Step 2: Check code formatting

Run: `dotnet format --verify-no-changes`

Expected: No formatting issues

### Step 3: Build in Release mode

Run: `dotnet build --configuration Release`

Expected: Build succeeds with 0 warnings

### Step 4: Run smoke test

Run: `cd Executable && dotnet run --no-build --configuration Release`

Expected: Application runs without errors, prompts for Todoist sync

### Step 5: Update CLAUDE.md documentation

In `CLAUDE.md`, update the "Meal Fallback Mechanism" section (around line 40):

**Replace:**
```markdown
Meals support fallback FoodGroupings when the primary grouping produces negative servings or has no solution:

\`\`\`csharp
Meal.WithFallbacks("Meal Name", macros, primaryGrouping, fallback1, fallback2)
\`\`\`
```

**With:**
```markdown
Meals support fallback FoodGroupings when the primary grouping produces negative servings or has no solution:

\`\`\`csharp
new Meal("Meal Name", macros, new FallbackChain(primaryGrouping, fallback1, fallback2))
// Or using predefined chains from FoodGroupings:
new Meal("Meal Name", macros, FoodGroupings.Ezekiel)
\`\`\`

The `FallbackChain` type makes fallback chains explicit in the type system, with properties:
- `Primary`: The first (preferred) grouping
- `Fallbacks`: Subsequent options if primary fails
- `All`: Complete array for iteration
- `Count`: Number of groupings in chain
```

### Step 6: Commit documentation update

```bash
git add CLAUDE.md
git commit -m "docs: update CLAUDE.md to document FallbackChain type"
```

### Step 7: Final commit with summary

```bash
git commit --allow-empty -m "feat: introduce FallbackChain type for explicit fallback chains

Summary of changes:
- Created FallbackChain record with validation (min 2 groupings)
- Added Meal constructor accepting FallbackChain
- Removed Meal.WithFallbacks method (now redundant)
- Updated FoodGroupings properties from FoodGrouping[] to FallbackChain
- Updated all TrainingWeek files to use new constructors
- Updated all tests
- Fixed typo: Ezekial → Ezekiel

Benefits:
- Type system now distinguishes single FoodGrouping from fallback chains
- Improved code clarity and discoverability
- Constructor validation prevents invalid chains
- Properties (Primary, Fallbacks, All) make intent explicit"
```

---

## Execution Complete

All tests pass, code is formatted, and documentation is updated. The FallbackChain type is now integrated throughout the codebase.
