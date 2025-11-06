using System.Text;
using SystemOfEquations.Data;

namespace SystemOfEquations;

internal record Phase(string Name, WeeklyMealsPrepPlan MealPrepPlan, TrainingWeek TrainingWeek)
{
    public Phase(string name, TrainingWeek trainingWeek)
        : this(name, WeeklyMealsPrepPlans.CreateMealPrepPlan(trainingWeek), trainingWeek)
    {
    }

    public override string ToString()
    {
        try
        {
            var sb = new StringBuilder();

            var totalActualCals = 0.0M;
            var totalActualMacros = new Macros(0, 0, 0);
            var totalActualFiber = 0.0M;
            var totalTargetMacros = new Macros(0, 0, 0);
            var hasAnyConversionFoods = false;

            foreach (var trainingDay in TrainingWeek.TrainingDays)
            {
                var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
                totalActualCals += trainingDay.ActualNutrients.Cals * daysPerWeek;
                totalActualMacros += trainingDay.ActualNutrients.Macros * daysPerWeek;
                totalActualFiber += trainingDay.ActualNutrients.Fiber * daysPerWeek;
                totalTargetMacros += trainingDay.TargetMacros * daysPerWeek;
                hasAnyConversionFoods |= trainingDay.HasConversionFoods;
            }

            if (hasAnyConversionFoods)
            {
                // Show both ACTUAL and TARGET when conversion foods are present
                sb.AppendLine("Week Average:");
                sb.AppendLine($"  ACTUAL: {totalActualCals / 7:F0} cals, {totalActualMacros / 7}, {totalActualFiber / 7:F1}g fiber");
                sb.AppendLine($"  TARGET: {totalTargetMacros / 7}");
            }
            else
            {
                // No conversion foods - show unlabeled average
                sb.AppendLine($"Ave per day: {totalActualCals / 7:F0} cals, {totalActualMacros / 7}, {totalActualFiber / 7:F1}g fiber");
            }

            sb.AppendLine();

            sb.AppendLine(Name);
            foreach (var trainingDay in TrainingWeek.TrainingDays)
            {
                sb.AppendLine(trainingDay.ToString());
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"{Name} > {ex.Message}");
        }
    }
}
