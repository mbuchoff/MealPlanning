using System.Text;

namespace SystemOfEquations;

internal record TrainingDay
{
    public TrainingDay(TrainingDayType trainingDayType, IEnumerable<Meal> meals)
    {
        TrainingDayType = trainingDayType;
        Meals = meals;

        var helpings = Meals.SelectMany(m => m.Helpings).ToList();
        TotalNutrients = (
            Cals: helpings.Sum(h => h.Servings * h.Food.NutritionalInformation.Cals),
            Macros: helpings.Sum(h => h.Servings * h.Food.NutritionalInformation.Macros));
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        var nutrients = TotalNutrients;
        sb.AppendLine($"{TrainingDayType} - {nutrients.Cals:F0} calories, {nutrients.Macros}");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }
        
        return sb.ToString();
    }

    public IEnumerable<Meal> Meals { get; }
    public TrainingDayType TrainingDayType { get; }
    public (double Cals, Macros Macros) TotalNutrients { get; }
}
