using System.Text;

namespace SystemOfEquations;

internal record TrainingDay(TrainingDayType TrainingDayType, IEnumerable<Meal> Meals)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        var cals = Meals.SelectMany(m => m.Helpings).Sum(h => h.Servings * h.Food.NutritionalInformation.Cals);
        sb.AppendLine($"{TrainingDayType} - {cals:F0} calories");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }
        
        return sb.ToString();
    }
}
