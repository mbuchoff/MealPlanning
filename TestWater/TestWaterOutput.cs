using SystemOfEquations;
using SystemOfEquations.Data;

// Quick test for water combination in totals
var brownRice = Foods.BrownRice_45_Grams * 2; // Should have 2.5 cups water
var wheatBerries = Foods.WheatBerries_45_Grams * 1; // Should have 2.6 cups water

var servings = new[] { brownRice, wheatBerries };

Console.WriteLine("Individual Servings:");
foreach (var serving in servings)
{
    Console.WriteLine($"- {serving.Name}:");
    foreach (var line in serving.ToOutputLines("  "))
    {
        Console.WriteLine(line);
    }
}

Console.WriteLine("\nExpanded Components:");
var allComponents = servings.SelectMany(s => s.GetComponentsForDisplay());
foreach (var component in allComponents)
{
    Console.WriteLine($"- {component}");
}

Console.WriteLine("\nCombined Components:");
var combinedComponents = allComponents.CombineLikeServings();
foreach (var component in combinedComponents)
{
    Console.WriteLine($"- {component}");
}

// Expected: One water line with ~5.1 cups (2.5 + 2.6)