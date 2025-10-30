namespace SystemOfEquations;

internal record MealPrepPlan(
    string Name,
    IEnumerable<FoodServing> CookingServings,
    IEnumerable<FoodServing> EatingServings,
    int MealCount);