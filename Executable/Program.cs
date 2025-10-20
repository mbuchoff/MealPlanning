using SystemOfEquations;
using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Todoist;

// Set your target average daily calories here
var targetDailyCalories = 3200M;

// Automatically calculate the required adjustment
// Note: The base MuscleGain2 is at 100% (no adjustment)
var baseTrainingWeek = new MuscleGain2();
var trainingWeek = baseTrainingWeek.ForTargetCalories(targetDailyCalories);

// Calculate the actual percentage for display purposes
var baseTotalCals = 0.0M;
foreach (var day in baseTrainingWeek.TrainingDays)
{
    baseTotalCals += day.TotalNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
}
var baseAverage = baseTotalCals / 7;

var adjustedTotalCals = 0.0M;
foreach (var day in trainingWeek.TrainingDays)
{
    adjustedTotalCals += day.TotalNutrients.Cals * day.TrainingDayType.DaysTraining.Count;
}
var adjustedAverage = adjustedTotalCals / 7;

var actualPercentChange = ((adjustedAverage / baseAverage) - 1) * 100;

var phase = new Phase($"{trainingWeek.Name}, adjusted to {targetDailyCalories:F0} cal/day ({actualPercentChange:+0.0;-0.0}%)", trainingWeek);

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
