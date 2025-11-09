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
        var actualNutrients = ActualNutrients;
        var actualString = $"{actualNutrients.Cals:F0} calories, {actualNutrients.Macros}, {actualNutrients.Fiber:F1}g fiber";

        if (HasConversionFoods)
        {
            // Show both ACTUAL and TARGET when conversion foods are present
            sb.AppendLine($"{TrainingDayType}:");
            var nutrientsLine = NutritionalFormatting.FormatWithOptionalTarget(
                actualString,
                TargetMacros.ToString(),
                HasConversionFoods,
                prefix: "  ");
            sb.AppendLine(nutrientsLine);
        }
        else
        {
            // No conversion foods - show unlabeled nutrients on same line as TrainingDayType
            sb.AppendLine($"{TrainingDayType}: {actualString}");
        }

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

    private Macros? _targetMacros;
    /// <summary>
    /// Gets the target macros for this training day (sum of all meal target macros).
    /// This represents the intended nutritional goals.
    /// </summary>
    public Macros TargetMacros
    {
        get
        {
            if (_targetMacros != null)
            {
                return _targetMacros;
            }

            _targetMacros = Meals.Sum(m => m.Macros);
            return _targetMacros;
        }
    }

    /// <summary>
    /// Indicates whether any meal in this training day contains conversion foods.
    /// </summary>
    public bool HasConversionFoods => Meals.Any(m => m.HasConversionFoods);
}
