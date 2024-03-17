using System.Text;

namespace SystemOfEquations;

internal record Phase(string Name, MealPrepPlan MealPrepPlan, TrainingWeek TrainingWeek)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Name);
        foreach (var trainingDay in TrainingWeek.TrainingDays)
        {
            sb.AppendLine(trainingDay.ToString());
        }
        return sb.ToString();
    }
}
