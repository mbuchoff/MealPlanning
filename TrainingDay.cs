using System.Text;

namespace SystemOfEquations;

internal record TrainingDay(string Name, IEnumerable<Meal> Meals)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        var cals = Meals.SelectMany(m => m.Helpings).Sum(h => h.Servings * h.Food.NutritionalInformation.Cals);
        sb.AppendLine($"{Name} - {cals} calories");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }

        return sb.ToString();
    }
}
