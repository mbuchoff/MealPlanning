using System.Text;

namespace SystemOfEquations;

public record TrainingDay(string Name, IEnumerable<Meal> Meals)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Name);
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }

        return sb.ToString();
    }
}
