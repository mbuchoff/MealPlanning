# Todoist Day Type Grouping Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Group Todoist eating meals under recurring day-type parent tasks (Crossfit day, Running day, Non-weight training day).

**Architecture:** Extract helper method that organizes meals by TrainingDayType, then modify AddPhaseAsync to create parent tasks and nest meals underneath. Follow strict TDD.

**Tech Stack:** .NET 9.0, xUnit, Todoist API

---

## Task 1: Add Helper Record Types

**Files:**
- Modify: `Executable/Todoist/TodoistService.cs:6-7` (after class declaration)

**Step 1: Add helper record types**

Add these records inside the `TodoistService` class (after line 6, before the `SyncAsync` method):

```csharp
private record DayTypeGroup(
    TrainingDayType TrainingDayType,
    string DueString,
    List<MealWithIndex> Meals);

private record MealWithIndex(
    int Index,
    Meal Meal,
    IEnumerable<FoodServing> Servings);
```

**Step 2: Verify it compiles**

Run: `dotnet build`
Expected: Build succeeds

**Step 3: Commit**

```bash
git add Executable/Todoist/TodoistService.cs
git commit -m "Add helper records for day-type grouping"
```

---

## Task 2: Implement GetDayTypeGroups Method (TDD)

**Files:**
- Create: `Test/GetDayTypeGroupsTests.cs`
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Write failing test for basic grouping structure**

Create `Test/GetDayTypeGroupsTests.cs`:

```csharp
using SystemOfEquations.Data;
using SystemOfEquations.Todoist;
using Xunit;

namespace SystemOfEquations.Test;

public class GetDayTypeGroupsTests
{
    [Fact]
    public void ReturnsThreeGroups_OnePerDayType()
    {
        // Arrange
        var phase = TrainingWeeks.MuscleGain2.ForTargetCalories(2000);

        // Act
        var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

        // Assert
        Assert.Equal(3, groups.Count);
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Crossfit day");
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Running day");
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Non-weight training day");
    }
}
```

**Step 2: Add test helper to expose private method**

In `Executable/Todoist/TodoistService.cs`, add at the end of the class (before the closing brace):

```csharp
// Test helper - makes GetDayTypeGroups accessible to tests
internal static IEnumerable<DayTypeGroup> GetDayTypeGroupsPublic(Phase phase)
    => GetDayTypeGroups(phase);
```

**Step 3: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~GetDayTypeGroupsTests.ReturnsThreeGroups_OnePerDayType"`
Expected: Compilation error - GetDayTypeGroups method doesn't exist

**Step 4: Write minimal implementation**

Add this method in `Executable/Todoist/TodoistService.cs` (before the test helper):

```csharp
private static IEnumerable<DayTypeGroup> GetDayTypeGroups(Phase phase)
{
    var allDayTypes = new[]
    {
        phase.TrainingWeek.XFitDay,
        phase.TrainingWeek.RunningDay,
        phase.TrainingWeek.NonworkoutDay
    };

    foreach (var trainingDay in allDayTypes)
    {
        yield return new DayTypeGroup(
            TrainingDayType: trainingDay.TrainingDayType,
            DueString: GetDueString(trainingDay.TrainingDayType),
            Meals: new List<MealWithIndex>());
    }
}
```

**Step 5: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~GetDayTypeGroupsTests.ReturnsThreeGroups_OnePerDayType"`
Expected: PASS

**Step 6: Commit**

```bash
git add Test/GetDayTypeGroupsTests.cs Executable/Todoist/TodoistService.cs
git commit -m "Add GetDayTypeGroups with empty meal lists"
```

---

## Task 3: Populate Meals in GetDayTypeGroups (TDD)

**Files:**
- Modify: `Test/GetDayTypeGroupsTests.cs`
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Write failing test for meal population**

Add to `Test/GetDayTypeGroupsTests.cs`:

```csharp
[Fact]
public void IncludesPrepareAsNeededMeals_WithAllServings()
{
    // Arrange
    var phase = TrainingWeeks.MuscleGain2.ForTargetCalories(2000);

    // Act
    var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

    // Assert - each group should have meals
    Assert.All(groups, g => Assert.NotEmpty(g.Meals));

    // Verify PrepareAsNeeded meals are included
    var xfitGroup = groups.First(g => g.TrainingDayType.ToString() == "Crossfit day");
    var prepareAsNeededMeals = phase.TrainingWeek.XFitDay.Meals
        .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
        .ToList();

    Assert.True(prepareAsNeededMeals.Count > 0, "Test setup requires PrepareAsNeeded meals");
    Assert.True(xfitGroup.Meals.Count >= prepareAsNeededMeals.Count);
}

[Fact]
public void MealIndices_ResetPerDayType()
{
    // Arrange
    var phase = TrainingWeeks.MuscleGain2.ForTargetCalories(2000);

    // Act
    var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

    // Assert - each group's meals start at index 1
    foreach (var group in groups.Where(g => g.Meals.Any()))
    {
        Assert.Equal(1, group.Meals.First().Index);

        // Verify sequential numbering
        var indices = group.Meals.Select(m => m.Index).ToList();
        Assert.Equal(Enumerable.Range(1, indices.Count), indices);
    }
}
```

**Step 2: Run tests to verify they fail**

Run: `dotnet test --filter "FullyQualifiedName~GetDayTypeGroupsTests"`
Expected: Both new tests FAIL - meals are empty

**Step 3: Implement meal population**

Replace the `GetDayTypeGroups` method in `Executable/Todoist/TodoistService.cs`:

```csharp
private static IEnumerable<DayTypeGroup> GetDayTypeGroups(Phase phase)
{
    var allDayTypes = new[]
    {
        phase.TrainingWeek.XFitDay,
        phase.TrainingWeek.RunningDay,
        phase.TrainingWeek.NonworkoutDay
    };

    foreach (var trainingDay in allDayTypes)
    {
        var mealsForDay = new List<MealWithIndex>();
        int mealIndex = 1;

        // Collect PrepareAsNeeded meals
        foreach (var meal in trainingDay.Meals
            .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
        {
            mealsForDay.Add(new MealWithIndex(
                Index: mealIndex++,
                Meal: meal,
                Servings: meal.Servings.Where(s => !s.IsConversion)));
        }

        // Collect PrepareInAdvance meals with AtEatingTime servings
        foreach (var meal in trainingDay.Meals
            .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance
                     && m.Servings.Any(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime)))
        {
            mealsForDay.Add(new MealWithIndex(
                Index: mealIndex++,
                Meal: meal,
                Servings: meal.Servings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime && !s.IsConversion)));
        }

        yield return new DayTypeGroup(
            TrainingDayType: trainingDay.TrainingDayType,
            DueString: GetDueString(trainingDay.TrainingDayType),
            Meals: mealsForDay);
    }
}
```

**Step 4: Run tests to verify they pass**

Run: `dotnet test --filter "FullyQualifiedName~GetDayTypeGroupsTests"`
Expected: All 3 tests PASS

**Step 5: Commit**

```bash
git add Test/GetDayTypeGroupsTests.cs Executable/Todoist/TodoistService.cs
git commit -m "Populate meals in GetDayTypeGroups with proper indexing"
```

---

## Task 4: Add AddMealSubtask Helper Method

**Files:**
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Add AddMealSubtask method**

Add this method in `Executable/Todoist/TodoistService.cs` (after `AddMealQuantitySubtask`, around line 195):

```csharp
private static async Task AddMealSubtask(
    TodoistTask dayTypeParentTask,
    MealWithIndex mealWithIndex,
    ProgressTracker progress)
{
    var content = $"{mealWithIndex.Index} - {mealWithIndex.Meal.Name}";

    var mealTask = await AddTaskAsync(
        content,
        description: null,
        dueString: null,
        parentId: dayTypeParentTask.Id,
        projectId: null,
        isCollapsed: true);
    progress.IncrementProgress();

    await UpdateTaskCollapsedAsync(mealTask.Id, collapsed: true);
    progress.IncrementProgress();

    // For PrepareInAdvance meals, add food grouping name as first subtask
    if (mealWithIndex.Meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
    {
        await AddTaskAsync(
            mealWithIndex.Meal.FoodGrouping.Name,
            description: null,
            dueString: null,
            parentId: mealTask.Id,
            projectId: null);
        progress.IncrementProgress();
    }

    // Add servings as subtasks
    await Task.WhenAll(mealWithIndex.Servings.Select(s => AddServingAsync(mealTask, s, progress)));

    // Add nutritional comment
    var comment = TodoistServiceHelper.GenerateNutritionalComment(mealWithIndex.Servings);
    await AddCommentAsync(mealTask.Id, comment);
    progress.IncrementProgress();
}
```

**Step 2: Verify it compiles**

Run: `dotnet build`
Expected: Build succeeds

**Step 3: Commit**

```bash
git add Executable/Todoist/TodoistService.cs
git commit -m "Add AddMealSubtask helper method"
```

---

## Task 5: Update CalculateTotalOperations for Day Type Parents (TDD)

**Files:**
- Create: `Test/CalculateTotalOperationsTests.cs`
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Write failing test for operation counting**

Create `Test/CalculateTotalOperationsTests.cs`:

```csharp
using SystemOfEquations.Data;
using SystemOfEquations.Todoist;
using Xunit;

namespace SystemOfEquations.Test;

public class CalculateTotalOperationsTests
{
    [Fact]
    public void IncludesDayTypeParentOperations()
    {
        // Arrange
        var phase = TrainingWeeks.MuscleGain2.ForTargetCalories(2000);

        // Act
        var totalOps = TodoistService.CalculateTotalOperationsPublic(phase, 0, 0);

        // Assert - should include operations for 3 day-type parents (create + collapse each)
        // This is a smoke test - just verify it doesn't crash and returns reasonable number
        Assert.True(totalOps > 6, "Should include at least 6 ops for day-type parents (3 * 2)");
    }
}
```

**Step 2: Add test helper to expose private method**

In `Executable/Todoist/TodoistService.cs`, add to the test helpers section at the end:

```csharp
// Test helper - makes CalculateTotalOperations accessible to tests
internal static int CalculateTotalOperationsPublic(Phase phase, int eatingTasksToDelete, int cookingTasksToDelete)
    => CalculateTotalOperations(phase, eatingTasksToDelete, cookingTasksToDelete);
```

**Step 3: Run test to verify baseline**

Run: `dotnet test --filter "FullyQualifiedName~CalculateTotalOperationsTests"`
Expected: PASS (baseline - we'll update counts next)

**Step 4: Update CalculateTotalOperations to include day-type parent operations**

In `Executable/Todoist/TodoistService.cs`, modify `CalculateTotalOperations` method. Find the section that counts eating meals (around line 87-103) and replace it with:

```csharp
// Day-type parent tasks (3 groups: XFit, Running, NonWorkout)
operations += 3 * 2; // Each group: parent task + collapse

// Eating meals from PrepareInAdvance meals (AtEatingTime servings)
var eatingTasksFromPrepMeals = new[]
{
    phase.TrainingWeek.XFitDay,
    phase.TrainingWeek.RunningDay,
    phase.TrainingWeek.NonworkoutDay,
}.SelectMany(trainingDay => trainingDay.Meals
    .Where(meal => meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance &&
                  meal.Servings.Any(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime)))
    .ToList();

foreach (var meal in eatingTasksFromPrepMeals)
{
    operations++; // Meal task
    operations++; // Collapse
    operations++; // Food grouping name subtask
    var eatingServings = meal.Servings.Where(s => s.AddWhen == FoodServing.AddWhenEnum.AtEatingTime && !s.IsConversion);
    operations += eatingServings.Sum(s => TodoistServiceHelper.CountTodoistOperations(s));
    operations++; // Comment
}

// Eating meals (PrepareAsNeeded)
var eatingMeals = new[]
{
    phase.TrainingWeek.XFitDay,
    phase.TrainingWeek.RunningDay,
    phase.TrainingWeek.NonworkoutDay,
}.SelectMany(trainingDay => trainingDay.Meals
    .Where(meal => meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded))
    .ToList();

foreach (var meal in eatingMeals)
{
    operations++; // Meal task
    operations++; // Collapse
    operations += meal.Servings.Where(s => !s.IsConversion).Sum(s => TodoistServiceHelper.CountTodoistOperations(s));
    operations++; // Comment
}
```

**Step 5: Run test to verify it still passes**

Run: `dotnet test --filter "FullyQualifiedName~CalculateTotalOperationsTests"`
Expected: PASS

**Step 6: Run all tests**

Run: `dotnet test`
Expected: All tests PASS

**Step 7: Commit**

```bash
git add Test/CalculateTotalOperationsTests.cs Executable/Todoist/TodoistService.cs
git commit -m "Update CalculateTotalOperations for day-type parents"
```

---

## Task 6: Modify AddPhaseAsync to Use Day Type Grouping

**Files:**
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Replace eating task creation logic in AddPhaseAsync**

In `Executable/Todoist/TodoistService.cs`, find the `AddPhaseAsync` method (line 197). Replace the section that creates eating tasks (lines 213-313) with:

```csharp
// Get day-type groups and create parent tasks with nested meals
var dayTypeGroups = GetDayTypeGroups(phase).ToList();

systemTasks.AddRange(dayTypeGroups.Select((group, groupIdx) => Task.Run(async () =>
{
    var eatingProject = await eatingProjectTask;

    // Create parent task for the day type
    var dayTypeParentTask = await AddTaskAsync(
        content: group.TrainingDayType.ToString(),
        description: null,
        dueString: group.DueString,
        parentId: null,
        eatingProject.Id,
        isCollapsed: true,
        order: groupIdx + 1);
    progress.IncrementProgress();

    await UpdateTaskCollapsedAsync(dayTypeParentTask.Id, collapsed: true);
    progress.IncrementProgress();

    // Create meal subtasks
    foreach (var mealWithIndex in group.Meals)
    {
        await AddMealSubtask(dayTypeParentTask, mealWithIndex, progress);
    }
})));
```

**Step 2: Verify it compiles**

Run: `dotnet build`
Expected: Build succeeds

**Step 3: Run all tests**

Run: `dotnet test`
Expected: All tests PASS

**Step 4: Commit**

```bash
git add Executable/Todoist/TodoistService.cs
git commit -m "Use day-type grouping in AddPhaseAsync"
```

---

## Task 7: Integration Testing with Manual Verification

**Files:**
- None (manual testing)

**Step 1: Check for Todoist API key**

Run: `dotnet user-secrets list --project Executable`
Expected: Should show "TodoistApiKey" if configured

If not configured and you have a Todoist account:
```bash
dotnet user-secrets set "TodoistApiKey" "your-api-key" --project Executable
```

**Step 2: Run the executable (dry run - review output)**

Run: `cd Executable && dotnet run --no-build`
Expected: Application runs and prompts for Todoist sync

- When prompted, type "no" to skip actual sync
- Review console output to verify structure looks correct

**Step 3: Optional - Test actual Todoist sync**

⚠️ **Only if you want to test against real Todoist:**

Run: `cd Executable && dotnet run --no-build`
- When prompted, type "yes"
- Check Todoist "Eating" project for:
  - 3 parent tasks (Crossfit day, Running day, Non-weight training day)
  - Each parent has recurring schedule
  - Meals nested under correct parent
  - Meal numbers reset per day type (1, 2, 3...)

**Step 4: Run formatting check**

Run: `dotnet format --verify-no-changes`
Expected: No formatting issues

If formatting issues found:
```bash
dotnet format
git add -A
git commit -m "Apply code formatting"
```

**Step 5: Final test run**

Run: `dotnet test`
Expected: All tests PASS

**Step 6: Final commit if any changes**

```bash
git status
# If any changes, commit them
```

---

## Task 8: Cleanup Test Helpers (Optional)

**Files:**
- Modify: `Executable/Todoist/TodoistService.cs`

**Step 1: Remove public test helpers**

The test helpers (`GetDayTypeGroupsPublic`, `CalculateTotalOperationsPublic`) expose internal methods for testing. If you prefer to keep them internal:

Option A: Keep them (easier testing, slight pollution of production code)
Option B: Remove them and use reflection in tests (cleaner production code, more complex tests)

**Recommendation:** Keep them. They're marked `internal` and only used by tests in the same assembly.

---

## Verification Checklist

Before marking complete:

- [ ] All tests pass (`dotnet test`)
- [ ] Code formatting clean (`dotnet format --verify-no-changes`)
- [ ] Build succeeds (`dotnet build`)
- [ ] All changes committed
- [ ] Manual verification in Todoist (if testing against real API)

---

## Notes

**Key Implementation Details:**
- `GetDayTypeGroups` uses `mealIndex++` to ensure sequential numbering per day type
- `AddMealSubtask` handles both PrepareAsNeeded (no food grouping subtask) and PrepareInAdvance (includes food grouping name)
- Operation counting includes 2 ops per day-type group (parent + collapse) plus existing meal operations
- Test helpers are marked `internal` - visible to test assembly but not external consumers

**Testing Strategy:**
- Unit tests for `GetDayTypeGroups` verify grouping logic
- Operation counting test is a smoke test (hard to predict exact count without mocking)
- Manual integration testing required for Todoist API behavior

**Potential Issues:**
- If no meals exist for a day type, an empty parent task will be created (by design)
- Progress tracker counts must match `CalculateTotalOperations` exactly or progress bar will be inaccurate
