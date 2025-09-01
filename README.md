# System of Equations - Meal Planning

[![CI/CD Pipeline](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/ci.yml/badge.svg)](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/ci.yml)
[![Run Tests](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/tests.yml/badge.svg)](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/tests.yml)

A .NET application for meal planning and nutritional calculations using a system of equations to balance macronutrients.

## Features

- Calculate meal plans based on macronutrient targets (protein, fat, carbohydrates)
- Support for different training day types (CrossFit, Running, Non-workout)
- Automatic water calculation for cooking
- Integration with Todoist for meal prep task management
- Flexible food serving definitions with nutritional equivalences

## Project Structure

- `Executable/` - Main application
  - `Data/` - Food definitions and training week configurations
  - `Todoist/` - Todoist API integration
- `Test/` - Unit tests

## Building and Running

### Prerequisites

- .NET 9.0 SDK or later

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run Application

```bash
cd Executable
dotnet run
```

## Recent Changes

### Food Serving Refactoring

The codebase has been refactored to better separate food types from serving sizes:

- **Before**: Foods like `PumpkinSeeds_30_Grams` and `PumpkinSeeds_1_Scoop` were defined separately
- **After**: Foods are defined once with their nutritional equivalences (e.g., 30g = 0.25 cups)

This eliminates redundancy and makes it clear that different serving sizes represent the same food.

## Testing

The project includes comprehensive unit tests covering:
- Nutritional calculations
- Meal planning logic
- Water calculations for cooking
- Food serving conversions

Tests run automatically on:
- Push to main branch
- Pull requests
- Multiple OS platforms (Ubuntu, Windows, macOS)

## Contributing

1. Create a feature branch
2. Make your changes
3. Ensure all tests pass (`dotnet test`)
4. Submit a pull request

## License

[Add license information]