namespace SystemOfEquations;

internal record MealPrepPlan(string Name, IEnumerable<FoodServing> Servings, int MealCount);