using SystemOfEquations;
using SystemOfEquations.Data;

var phase = Phases.MuscleGain3PlusPercent(percent: 5);

Console.WriteLine(phase);
Console.WriteLine(phase.MealPrepPlan);

Console.WriteLine();
Console.Write("Pushing to todoist...");
var todoist = new Todoist();
await todoist.PushAsync(phase);
Console.WriteLine(" Done");
