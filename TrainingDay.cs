using System.Text;

namespace SystemOfEquations;

internal record TrainingDay(TrainingDay.TrainingTypeEnum TrainingType, IEnumerable<Meal> Meals)
{
    public enum TrainingTypeEnum { CrossfitDay, NonWeightTrainingDay, RunningDay }

    public string GetTrainingTypeAsString() => TrainingType switch
    {
        TrainingTypeEnum.CrossfitDay => "Crossfit day",
        TrainingTypeEnum.NonWeightTrainingDay => "Non-weight training day",
        TrainingTypeEnum.RunningDay => "Running day",
        _ => throw new Exception($"{nameof(TrainingTypeEnum)} not found"),
    };

    public override string ToString()
    {
        var sb = new StringBuilder();
        var cals = Meals.SelectMany(m => m.Helpings).Sum(h => h.Servings * h.Food.NutritionalInformation.Cals);
        sb.AppendLine($"{GetTrainingTypeAsString()} - {cals:F0} calories");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }
        
        return sb.ToString();
    }
}
