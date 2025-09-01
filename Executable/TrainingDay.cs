using System.Text;

namespace SystemOfEquations;

internal record TrainingDay
{
    public TrainingDay(TrainingDayType trainingDayType, IEnumerable<Meal> meals)
    {
        TrainingDayType = trainingDayType;
        Meals = meals;
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

    private (decimal Cals, Macros Macros, decimal Fiber)? _totalNutrients;
    public (decimal Cals, Macros Macros, decimal Fiber) TotalNutrients
    {
        get
        {
            try
            {
                if (_totalNutrients != null)
                {
                    return _totalNutrients.Value;
                }

                var servings = Meals.SelectMany(m => m.Servings).ToList();
                _totalNutrients = (
                    Cals: servings.Sum(s => s.NutritionalInformation.Cals),
                    Macros: servings.Sum(s => s.NutritionalInformation.Macros),
                    Fiber: servings.Sum(s => s.NutritionalInformation.CFiber));

                return _totalNutrients.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"{TrainingDayType} > {ex.Message}");
            }
        }
    }
}
