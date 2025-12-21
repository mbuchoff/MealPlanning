using SystemOfEquations;
using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Data.TrainingWeeks.MuscleGain3;
using SystemOfEquations.Todoist;

// Set your target average daily calories here
const decimal targetDailyCalories = 3600M;

// Automatically calculate the required adjustment
var baseTrainingWeek = new MuscleGain2();
var trainingWeek = baseTrainingWeek.ForTargetCalories(targetDailyCalories);

// Calculate the actual percentage for display purposes
var baseTotalCals = 0.0M;
foreach (var day in baseTrainingWeek.TrainingDays)
{
    baseTotalCals += day.ActualNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
}
var baseAverage = baseTotalCals / 7;

var adjustedTotalCals = 0.0M;
foreach (var day in trainingWeek.TrainingDays)
{
    adjustedTotalCals += day.ActualNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
}
var adjustedAverage = adjustedTotalCals / 7;

var actualPercentChange = ((adjustedAverage / baseAverage) - 1) * 100;

var phase = new Phase($"{trainingWeek.Name}, adjusted to {targetDailyCalories:F0} cal/day ({actualPercentChange:+0.0;-0.0}%)", trainingWeek);

// Check if running interactively (no args) or in scripted mode (with args)
if (args.Length == 0 && Console.IsInputRedirected == false)
{
    // Interactive mode - launch the TUI navigator
    await InteractiveNavigator.RunAsync(phase);
}
else
{
    // Scripted mode - use original behavior
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
}
