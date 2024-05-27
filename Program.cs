using SystemOfEquations;
using SystemOfEquations.Data;

var phase = Phases.MuscleGain3PlusPercent(percent: 5);

Console.WriteLine(phase);
Console.WriteLine(phase.MealPrepPlan);

Console.WriteLine();
Console.Write("Syncing with Todoist...");
var todoist = new Todoist();
await todoist.SyncAsync(phase);
Console.WriteLine(" Done");
