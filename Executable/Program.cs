using SystemOfEquations;
using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Todoist;

// 2900
var percentIncrease = -9.65M;
var trainingWeek = new MuscleGain2().PlusPercent(100 + percentIncrease);
var phase = new Phase($"{trainingWeek.Name}, plus {percentIncrease}%", trainingWeek);

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
