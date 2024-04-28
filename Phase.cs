using System.Text;

namespace SystemOfEquations;

internal record Phase(string Name, MealPrepPlan MealPrepPlan, TrainingWeek TrainingWeek)
{
    public override string ToString()
    {
        var sb = new StringBuilder();

        var totalCals = 0.0;
        var totalMacros = new Macros(0, 0, 0);

        foreach(var trainingDay in TrainingWeek.TrainingDays)
        {
            var daysPerWeek = trainingDay.TrainingDayType.DaysPerWeek;
            totalCals += trainingDay.TotalNutrients.Cals * daysPerWeek;
            totalMacros += trainingDay.TotalNutrients.Macros * daysPerWeek;
        }

        sb.AppendLine($"Ave per day: {totalCals / 7:F0} cals, {totalMacros / 7}");
        sb.AppendLine();

        sb.AppendLine(Name);
        foreach (var trainingDay in TrainingWeek.TrainingDays)
        {
            sb.AppendLine(trainingDay.ToString());
        }
        return sb.ToString();
    }
}
