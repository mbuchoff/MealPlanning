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

    public FoodGrouping[] FoodGroupings => _foodGroupings;

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

    public Meal CloneWithTweakedMacros(decimal pPercent, decimal fPercent, decimal cPercent)
    {
        var tweakedMacros = Macros.CloneWithTweakedMacros(pPercent, fPercent, cPercent);
        // Preserve ALL fallback options, not just the one that was used
        return WithFallbacks(Name, tweakedMacros, FoodGroupings);
    }
}

internal static class MealExtensions
{
    public static IEnumerable<(Meal Meal, int MealCount)> SumWithSameFoodGrouping(this IEnumerable<Meal> meals, int daysPerWeek)
    {
        // Group by the complete set of fallback FoodGroupings, not just the one that was used
        var mealGroups = meals.GroupBy(m => m.FoodGroupings, new FoodGroupingArrayComparer());
        var summedMeals = mealGroups.Select(mealGroup =>
        {
            var mealCount = mealGroup.Count() * daysPerWeek;
            var totalMacros = mealGroup.Sum(m => m.Macros) * daysPerWeek;

            // Scale ALL fallback FoodGroupings, not just the one that was used
            var scaledFoodGroupings = mealGroup.Key.Select(fg =>
            {
                var scaledStaticServings = fg.StaticServings
                    .Select(s => s * mealCount)
                    .ToList();

                return new FoodGrouping(
                    fg.Name,
                    scaledStaticServings,
                    fg.PFood,
                    fg.FFood,
                    fg.CFood,
                    fg.PreparationMethod);
            }).ToArray();

            // Create meal with ALL scaled fallback options preserved
            var meal = Meal.WithFallbacks(scaledFoodGroupings[0].Name, totalMacros, scaledFoodGroupings);

            return (meal, mealCount);
        });
        return summedMeals;
    }

    private class FoodGroupingArrayComparer : IEqualityComparer<FoodGrouping[]>
    {
        public bool Equals(FoodGrouping[]? x, FoodGrouping[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;

            // Compare each FoodGrouping in the arrays
            for (int i = 0; i < x.Length; i++)
            {
                // Use reference equality since FoodGroupings should be the same instances
                if (!ReferenceEquals(x[i], y[i])) return false;
            }
            return true;
        }

        public int GetHashCode(FoodGrouping[] obj)
        {
            if (obj == null) return 0;
            // Use hash code of first element for simplicity
            return obj.Length > 0 ? obj[0].GetHashCode() : 0;
        }
    }
}
