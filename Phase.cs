using System.Text;

namespace SystemOfEquations;

internal record Phase(string Name, MealPrepPlan MealPrepPlan, TrainingWeek TrainingWeek)
{
    public override string ToString()
    {
        var sb = new StringBuilder();

        var totalCals = 0.0;
        var totalMacros = new Macros(0, 0, 0);
        var totalFiber = 0.0;

        foreach(var trainingDay in TrainingWeek.TrainingDays)
        {
            var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
            totalCals += trainingDay.TotalNutrients.Cals * daysPerWeek;
            totalMacros += trainingDay.TotalNutrients.Macros * daysPerWeek;
            totalFiber += trainingDay.TotalNutrients.Fiber * daysPerWeek;
        }

        sb.AppendLine($"Ave per day: {totalCals / 7:F0} cals, {totalMacros / 7}, {totalFiber / 7:F1}g fiber");
        sb.AppendLine();

        sb.AppendLine(Name);
        foreach (var trainingDay in TrainingWeek.TrainingDays)
        {
            sb.AppendLine(trainingDay.ToString());
        }
        return sb.ToString();
    }
}
