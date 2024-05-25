using System.Text;

namespace SystemOfEquations;

internal record TrainingDay
{
    public TrainingDay(TrainingDayType trainingDayType, IEnumerable<Meal> meals)
    {
        TrainingDayType = trainingDayType;
        Meals = meals;

        var invalidMeal = Meals.FirstOrDefault(m => m.ErrorState != null);

        if (invalidMeal != null)
        {
            throw new Exception($"{TrainingDayType} > {invalidMeal.Name} > {invalidMeal.ErrorState}");
        }

        var helpings = Meals.SelectMany(m => m.Helpings).ToList();
        TotalNutrients = (
            Cals: helpings.Sum(h => h.Servings * h.Food.NutritionalInformation.Cals),
            Macros: helpings.Sum(h => h.Servings * h.Food.NutritionalInformation.Macros),
            Fiber: helpings.Sum(h => h.Servings * h.Food.NutritionalInformation.CFiber));
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        var nutrients = TotalNutrients;
        sb.AppendLine($"{TrainingDayType}: {nutrients.Cals:F0} calories, {nutrients.Macros}, {nutrients.Fiber:F1}g fiber");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }
        
        return sb.ToString();
    }

    public IEnumerable<Meal> Meals { get; }
    public TrainingDayType TrainingDayType { get; }
    public (double Cals, Macros Macros, double Fiber) TotalNutrients { get; }
}
