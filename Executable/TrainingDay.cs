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
        var nutrients = ActualNutrients;
        sb.AppendLine($"{TrainingDayType}: {nutrients.Cals:F0} calories, {nutrients.Macros}, {nutrients.Fiber:F1}g fiber");
        foreach (var meal in Meals)
        {
            sb.AppendLine(meal.ToString());
        }

        return sb.ToString();
    }

    public IEnumerable<Meal> Meals { get; }
    public TrainingDayType TrainingDayType { get; }

    private (decimal Cals, Macros Macros, decimal Fiber)? _actualNutrients;
    /// <summary>
    /// Gets the actual consumed nutrients (excluding conversion foods).
    /// This represents what is actually eaten.
    /// </summary>
    public (decimal Cals, Macros Macros, decimal Fiber) ActualNutrients
    {
        get
        {
            try
            {
                if (_actualNutrients != null)
                {
                    return _actualNutrients.Value;
                }

                // Only include non-conversion servings (what's actually consumed)
                var servings = Meals.SelectMany(m => m.Servings.Where(s => !s.IsConversion)).ToList();
                _actualNutrients = (
                    Cals: servings.Sum(s => s.NutritionalInformation.Cals),
                    Macros: servings.Sum(s => s.NutritionalInformation.Macros),
                    Fiber: servings.Sum(s => s.NutritionalInformation.CFiber));

                return _actualNutrients.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"{TrainingDayType} > {ex.Message}");
            }
        }
    }

    [Obsolete("Use ActualNutrients instead. TotalNutrients includes conversion foods which aren't actually consumed.")]
    public (decimal Cals, Macros Macros, decimal Fiber) TotalNutrients => ActualNutrients;
}
