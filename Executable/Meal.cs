using System.Text;
using SystemOfEquations.Data;
using SystemOfEquations.Extensions;

namespace SystemOfEquations;

public class Meal
{
    public Meal(string name, Macros macros, FoodGrouping foodGrouping)
    {
        Name = name;
        Macros = macros;
        _foodGroupings = [foodGrouping];
        ActualFoodGrouping = foodGrouping;
    }

    public static Meal WithFallbacks(string name, Macros macros, params FoodGrouping[] foodGroupings)
    {
        return new Meal(name, macros, foodGroupings);
    }

    private Meal(string name, Macros macros, params FoodGrouping[] foodGroupings)
    {
        Name = name;
        Macros = macros;
        _foodGroupings = foodGroupings ?? throw new ArgumentNullException(nameof(foodGroupings));
        if (foodGroupings.Length == 0)
            throw new ArgumentException("At least one FoodGrouping must be provided", nameof(foodGroupings));
    }

    private readonly FoodGrouping[] _foodGroupings;
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Name}: {FoodGrouping.Name}");

        if (FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
        {
            foreach (var serving in Servings.Where(s => !s.IsConversion))
            {
                sb.AppendLine(serving.ToString());
            }
        }

        return sb.ToString();
    }

    public string Name { get; private set; }
    public FoodGrouping FoodGrouping
    {
        get
        {
            // Ensure ActualFoodGrouping is populated
            if (ActualFoodGrouping == null && _foodGroupings != null && _foodGroupings.Length > 0)
            {
                // Force calculation if not yet done
                _ = Servings;
            }
            return ActualFoodGrouping ?? throw new InvalidOperationException("FoodGrouping has not been determined yet");
        }
    }
    public FoodGrouping? ActualFoodGrouping { get; private set; }

    private IEnumerable<FoodServing>? _servings = null;
    public IEnumerable<FoodServing> Servings
    {
        get
        {
            if (_servings != null)
            {
                return _servings;
            }

            Exception? lastException = null;

            foreach (var foodGrouping in _foodGroupings)
            {
                try
                {
                    var calculatedServings = TryCalculateServings(foodGrouping);
                    _servings = calculatedServings;
                    ActualFoodGrouping = foodGrouping;
                    return _servings;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    continue;
                }
            }

            throw lastException ?? new Exception("No FoodGroupings provided");
        }
    }

    private IEnumerable<FoodServing> TryCalculateServings(FoodGrouping foodGrouping)
    {
        var pMacros = foodGrouping.PFood.NutritionalInformation.Macros;
        var fMacros = foodGrouping.FFood.NutritionalInformation.Macros;
        var cMacros = foodGrouping.CFood.NutritionalInformation.Macros;
        var remainingMacros = Macros - foodGrouping.StaticServings.Sum(s => s.NutritionalInformation.Macros);

        var solution = Equation.Solve(
            new(pMacros.P, fMacros.P, cMacros.P, remainingMacros.P),
            new(pMacros.F, fMacros.F, cMacros.F, remainingMacros.F),
            new(pMacros.C, fMacros.C, cMacros.C, remainingMacros.C));

        if (solution == null)
        {
            throw new Exception("No solution");
        }

        (var pFoodServings, var fFoodServings, var cFoodServings) = solution.Value;

        var servings = foodGrouping.StaticServings.Append(
        [
            foodGrouping.PFood * pFoodServings,
            foodGrouping.FFood * fFoodServings,
            foodGrouping.CFood * cFoodServings,
        ]);

        foreach (var serving in servings)
        {
            if (serving.NutritionalInformation.ServingUnits < 0 && !serving.IsConversion)
            {
                throw new Exception($"{Name} > {serving.NutritionalInformation.ServingUnits:F2} servings in {serving.Name}.");
            }
        }

        return servings;
    }

    public Macros Macros { get; private set; }
    public NutritionalInformation NutritionalInformation => Servings
        .Select(s => s.NutritionalInformation)
        .Sum(1, ServingUnits.Meal);

    public Meal CloneWithTweakedMacros(decimal pPercent, decimal fPercent, decimal cPercent) =>
        new(Name, Macros.CloneWithTweakedMacros(pPercent, fPercent, cPercent), FoodGrouping);
}

internal static class MealExtensions
{
    public static IEnumerable<(Meal Meal, int MealCount)> SumWithSameFoodGrouping(this IEnumerable<Meal> meals, int daysPerWeek)
    {
        var mealGroups = meals.GroupBy(m => m.FoodGrouping);
        var summedMeals = mealGroups.Select(mealGroup =>
        {
            var mealCount = mealGroup.Count() * daysPerWeek;
            var totalMacros = mealGroup.Sum(m => m.Macros);

            // Create a new FoodGrouping with scaled static servings
            var scaledStaticServings = mealGroup.Key.StaticServings
                .Select(s => s * mealCount)
                .ToList();

            var scaledFoodGrouping = new FoodGrouping(
                mealGroup.Key.Name,
                scaledStaticServings,
                mealGroup.Key.PFood,
                mealGroup.Key.FFood,
                mealGroup.Key.CFood,
                mealGroup.Key.PreparationMethod);

            var meal = new Meal(
                mealGroup.Key.Name,
                totalMacros,
                scaledFoodGrouping);

            return (meal, mealCount);
        });
        return summedMeals;
    }
}
