# Todoist Day Type Grouping Design

**Date:** 2025-10-31
**Status:** Approved

## Problem

Current Todoist eating tasks are flat, showing meal entries like:
- "1 - Crossfit day - 1-3 hours before workout"
- "2 - Crossfit day - 1/2 shake during workout, 1/2 shake right after"
- "3 - Crossfit day - 40 minutes after workout"
- "1 - Running day - Breakfast"
- etc.

This makes it hard to see which meals belong to which day type at a glance.

## Solution

Group eating meals under day-type parent tasks:
- **Crossfit day** (recurring parent)
  - 1 - 1-3 hours before workout
  - 2 - 1/2 shake during workout, 1/2 shake right after
  - 3 - 40 minutes after workout
- **Running day** (recurring parent)
  - 1 - Breakfast
  - 2 - Lunch
- **Non-weight training day** (recurring parent)
  - 1 - Breakfast
  - etc.

### Key Decisions

1. **Day-type parents are recurring tasks** matching their meal schedules (e.g., "every mon,wed,fri")
2. **Meal numbering resets per day type** (1, 2, 3 for each group)
3. **All eating project meals grouped** (both PrepareAsNeeded and PrepareInAdvance with AtEatingTime servings)

## Architecture

### Data Structures

New helper records in `TodoistService.cs`:

```csharp
private record DayTypeGroup(
    TrainingDayType TrainingDayType,
    string DueString,
    List<MealWithIndex> Meals);

private record MealWithIndex(
    int Index,              // 1, 2, 3 (resets per day type)
    Meal Meal,
    IEnumerable<FoodServing> Servings);
```

### Helper Method

`GetDayTypeGroups(Phase phase)` extracts and organizes eating meals:
- Iterates through XFitDay, RunningDay, NonworkoutDay
- For each day type:
  - Collects PrepareAsNeeded meals (all servings)
  - Collects PrepareInAdvance meals (only AtEatingTime servings)
  - Numbers meals sequentially (1, 2, 3...)
- Returns `IEnumerable<DayTypeGroup>`

### Task Creation

Modified `AddPhaseAsync`:
1. Call `GetDayTypeGroups(phase)` to get structured data
2. For each group:
   - Create day-type parent task (e.g., "Crossfit day")
   - Set recurring due string (e.g., "every mon,wed,fri")
   - Collapse parent task
   - Create meal subtasks with format "{index} - {meal.Name}"
3. Each meal subtask includes servings and nutritional comment

New helper method `AddMealSubtask`:
- Creates meal task under day-type parent
- Adds food grouping name (for PrepareInAdvance meals)
- Adds servings as subtasks
- Adds nutritional comment

## Operation Count Updates

`CalculateTotalOperations` needs adjustment:
- Add 2 operations per day-type group (parent task + collapse)
- Existing meal operations unchanged (they become subtasks instead of top-level)

## Testing Strategy

Following TDD (RED-GREEN-REFACTOR):

### Unit Tests for `GetDayTypeGroups`
- Returns 3 groups (XFit, Running, NonWorkout)
- Meal indices reset per day type (1, 2, 3 for each)
- PrepareAsNeeded meals include all servings
- PrepareInAdvance meals include only AtEatingTime servings
- Meals without eating servings excluded

### Integration Tests
- Verify parent tasks created for each day type
- Verify meals nested under correct parent
- Verify meal numbering and naming format

## Implementation Steps

1. Write failing tests for `GetDayTypeGroups`
2. Implement helper records and method
3. Write failing tests for task nesting
4. Modify `AddPhaseAsync` to use grouping
5. Implement `AddMealSubtask` helper
6. Update `CalculateTotalOperations`
7. Verify all tests pass
8. Manual verification in Todoist

## Migration Notes

This is a breaking change to Todoist output structure:
- Old tasks will remain until next sync
- New sync will create grouped structure
- No data migration needed (tasks auto-deleted on sync)
