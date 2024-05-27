using SystemOfEquations.Data;
using SystemOfEquations.Todoist;

var phase = Phases.MuscleGain3PlusPercent(percent: 5);

Console.WriteLine(phase);
Console.WriteLine(phase.MealPrepPlan);

Console.WriteLine();
Console.Write("Syncing with Todoist...");
await TodoistService.SyncAsync(phase);
Console.WriteLine(" Done");
