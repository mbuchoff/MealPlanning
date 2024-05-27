using SystemOfEquations;
using SystemOfEquations.Data;

var phase = Phases.MuscleGain3PlusPercent(percent: 5);

Console.WriteLine(phase);
Console.WriteLine(phase.MealPrepPlan);

Console.WriteLine();
var todoist = new Todoist();
await todoist.PushAsync(phase);