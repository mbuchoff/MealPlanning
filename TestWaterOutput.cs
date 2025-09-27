using SystemOfEquations;
using SystemOfEquations.Data;

// Test the water display
var yeast = new FoodServing("nutritional yeast",
    new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
var gluten = new FoodServing("gluten",
    new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

var composite = new CompositeFoodServing(
    "Seitan",
    new NutritionalInformation(1, ServingUnits.None, 79, 13.52M, 0.655M, 3.38M, 0.75M),
    [yeast, gluten],
    new FoodServing.AmountWater(0, 0.1M)); // 0.1 cups of water per serving

// Get components for display
var components = composite.GetComponentsForDisplay().ToList();

Console.WriteLine("Components for display:");
foreach (var component in components)
{
    Console.WriteLine($"- {component}");
}

// Simulate what Todoist would receive
Console.WriteLine("\nTodoist task creation would show:");
foreach (var component in components)
{
    Console.WriteLine($"- {component.ToString()}");
}