# System of Equations - Meal Planning

[![CI/CD Pipeline](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/ci.yml/badge.svg)](https://github.com/mbuchoff/SystemOfEquations/actions/workflows/ci.yml)

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
