using System.Text;

namespace SystemOfEquations;

public record Phase(string Name, IEnumerable<TrainingDay> TrainingDays)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Name);
        foreach (var trainingDay in TrainingDays)
        {
            sb.AppendLine(trainingDay.ToString());
        }
        return sb.ToString();
    }
}
