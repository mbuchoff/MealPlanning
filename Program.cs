using SystemOfEquations.Data;
using SystemOfEquations.Todoist;

var phase = Phases.BasePlusPercent(percent: 36.7M);

Console.WriteLine(phase);
Console.WriteLine(phase.MealPrepPlan);

Console.WriteLine();
Console.WriteLine("Sync with Todoist? Type 'yes' to confirm.");
if (Console.ReadLine()?.Trim().Equals("yes", StringComparison.CurrentCultureIgnoreCase) == true)
{
    Console.WriteLine();
    Console.WriteLine("Syncing with Todoist...");
    await TodoistService.SyncAsync(phase);
    Console.WriteLine("Done");
}
