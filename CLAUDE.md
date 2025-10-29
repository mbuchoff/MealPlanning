# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A .NET 9.0 meal planning application that uses systems of equations to calculate optimal food servings based on macronutrient targets (protein, fat, carbohydrates). Integrates with Todoist for meal prep task management.

## Essential Commands

### Building and Testing

```bash
# Build the solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Check code formatting (CI requirement)
dotnet format --verify-no-changes
```

### Running the Application

```bash
cd Executable
dotnet run

# Run without building
dotnet run --no-build --configuration Release
```

The application prompts for Todoist sync confirmation (type "yes" to sync).

### User Secrets

The application uses .NET User Secrets for Todoist API credentials:

```bash
dotnet user-secrets set "TodoistApiKey" "your-api-key-here" --project Executable
```

## Architecture

### Core Calculation Flow

1. **TrainingWeek** defines multiple `TrainingDay` instances with different macronutrient targets
2. **TrainingDay** contains multiple `Meal` instances for that day type
3. **Meal** uses a `FoodGrouping` to solve for optimal food servings via system of equations
4. **FoodGrouping** defines:
   - Static servings (fixed amounts, e.g., oils, seasonings)
   - Three variable foods: PFood (protein-focused), FFood (fat-focused), CFood (carb-focused)
5. **Equation.Solve()** uses Cramer's rule to solve 3x3 system for P/F/C servings

### System of Equations

Each meal calculates servings by solving:
- P equation: `pFood.P * x + fFood.P * y + cFood.C * z = targetP`
- F equation: `pFood.F * x + fFood.F * y + cFood.F * z = targetF`
- C equation: `pFood.C * x + fFood.C * y + cFood.C * z = targetC`

Where x, y, z are the servings of PFood, FFood, and CFood respectively. See `Meal.TryCalculateServings()` in Executable/Meal.cs.

### Meal Fallback Mechanism

Meals support fallback FoodGroupings when the primary grouping produces negative servings or has no solution:

```csharp
Meal.WithFallbacks("Meal Name", macros, primaryGrouping, fallback1, fallback2)
```

The meal tries each FoodGrouping sequentially until finding one that works. This is critical for scaling (via `ForTargetCalories()`) since different calorie levels may require different food combinations.

### Composite Food Servings

`CompositeFoodServing` represents multi-ingredient foods (e.g., "Protein Cookie" = oat flour + protein powder + egg whites). When scaled, components scale independently unless marked as `StaticFoodServing` (which don't scale - used for seasonings, etc.).

### Meal Prep Planning

`WeeklyMealsPrepPlan` aggregates meals across the week:
- Groups identical meals (by FoodGrouping fallback chain, not just selected one)
- Scales quantities for batch cooking
- Excludes "PrepareAsNeeded" meals from prep list
- Converts ingredients to practical units (e.g., eggs to dozen)

### Todoist Integration

`TodoistService` syncs meal prep plans to Todoist:
- Creates parent task for the phase
- Adds subtasks for each meal prep item
- Adds comment with intended macros for verification
- Converts static servings to prep-friendly formats

## Key Files and Locations

- `Executable/Program.cs` - Entry point, sets target calories and creates Phase
- `Executable/Equation.cs` - System of equations solver using Cramer's rule
- `Executable/Meal.cs` - Meal calculation with fallback logic
- `Executable/FoodGrouping.cs` - Food group definitions with P/F/C foods
- `Executable/CompositeFoodServing.cs` - Multi-ingredient foods
- `Executable/StaticFoodServing.cs` - Non-scaling ingredient components
- `Executable/Data/Foods.cs` - All food definitions
- `Executable/Data/FoodGroupings.cs` - All food grouping definitions
- `Executable/Data/TrainingWeeks/` - Training week configurations
- `Executable/Todoist/TodoistService.cs` - Todoist API integration
- `Test/` - xUnit test suite

## Important Patterns

### Immutable Records

Most domain objects are immutable records. Modifications create new instances:

```csharp
public record Meal { ... }
public record FoodServing { ... }
```

### Operator Overloading

Extensive use of operators for domain operations:
- `FoodServing * decimal` - Scale serving
- `FoodGrouping + FoodServing` - Add static serving
- `Macros + Macros`, `Macros * decimal` - Arithmetic

### Extension Methods for Aggregation

- `IEnumerable<T>.Sum()` - Custom extension for nutritional summation
- See `Executable/Extensions/IEnumerableExtensions.cs`

### Lazy Calculation with Caching

`Meal.Servings` and `TrainingDay.TotalNutrients` calculate on first access and cache results.

## Testing Philosophy

- Comprehensive unit tests in `Test/` using xUnit
- Tests cover equation solving, fallback logic, scaling, and Todoist formatting
- CI enforces code formatting with `dotnet format`
- Smoke test verifies executable produces expected output
- `TreatWarningsAsErrors=true` in both projects

## CI/CD

GitHub Actions workflow (`.github/workflows/ci.yml`):
1. Checks code formatting (must pass)
2. Builds in Release mode
3. Runs all unit tests with coverage
4. Runs smoke test of executable
5. Uploads test results as artifacts

All tests must pass and code must be formatted before merge.

## Data Configuration

Training weeks are defined in `Executable/Data/TrainingWeeks/` as C# classes (e.g., `MuscleGain2.cs`). Each defines:
- Training day types (CrossFit, Running, Rest)
- Meals per day with macro targets
- Days per week for each training day type

To change meal plans, modify or create new TrainingWeek classes and update `Program.cs`.
