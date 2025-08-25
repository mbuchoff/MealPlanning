using System.Text;
using SystemOfEquations.Data;
using SystemOfEquations.Extensions;

namespace SystemOfEquations;

public class Meal(string name, Macros macros, FoodGrouping foodGrouping)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Name}: {FoodGrouping.Name}");

        if (FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
        {
            foreach (var helping in Helpings.Where(h => !h.Food.IsConversion))
            {
                sb.AppendLine(helping.ToString());
            }
        }

        return sb.ToString();
    }

    public string Name { get; } = name;
    public FoodGrouping FoodGrouping { get; } = foodGrouping;

    private IEnumerable<Helping>? _helpings = null;
    public IEnumerable<Helping> Helpings
    {
        get
        {
            if (_helpings != null)
            {
                return _helpings;
            }

            var pMacros = FoodGrouping.PFood.NutritionalInformation.Macros;
            var fMacros = FoodGrouping.FFood.NutritionalInformation.Macros;
            var cMacros = FoodGrouping.CFood.NutritionalInformation.Macros;
            var remainingMacros = Macros - FoodGrouping.StaticHelpings.Sum(h => h.Macros);

            var solution = Equation.Solve(
                new(pMacros.P, fMacros.P, cMacros.P, remainingMacros.P),
                new(pMacros.F, fMacros.F, cMacros.F, remainingMacros.F),
                new(pMacros.C, fMacros.C, cMacros.C, remainingMacros.C));

            if (solution == null)
            {
                throw new Exception("No solution");
            }
            else
            {
                (var pFoodServings, var fFoodServings, var cFoodServings) = solution.Value;

                _helpings = FoodGrouping.StaticHelpings.Append(
                [
                    new Helping(FoodGrouping.PFood, pFoodServings),
                    new Helping(FoodGrouping.FFood, fFoodServings),
                    new Helping(FoodGrouping.CFood, cFoodServings),
                ]);

                foreach (var helping in Helpings)
                {
                    if (helping.Servings < 0 && !helping.Food.IsConversion)
                    {
                        throw new Exception($"{Name} > {helping.Servings:F2} servings in {helping.Food.Name}.");
                    }
                }
            }

            return _helpings;
        }
    }

    public Macros Macros { get; } = macros;
    public NutritionalInformation NutritionalInformation => Helpings
        .Select(h => h.NutritionalInformation)
        .Sum(1, ServingUnits.Meal);

    public Meal CloneWithTweakedMacros(decimal pPercent, decimal fPercent, decimal cPercent) =>
        new(Name, Macros.CloneWithTweakedMacros(pPercent, fPercent, cPercent), FoodGrouping);
}

internal static class MealExtensions
{
    public static IEnumerable<Meal> SumWithSameFoodGrouping(this IEnumerable<Meal> meals, int daysPerWeek)
    {
        var mealGroups = meals.GroupBy(m => m.FoodGrouping);
        var summedMeals = mealGroups.Select(mealGroup =>
        {
            var mealCount = mealGroup.Count() * daysPerWeek;
            var totalMacros = mealGroup.Sum(m => m.Macros);
            
            // Create a new FoodGrouping with scaled static helpings
            var scaledStaticHelpings = mealGroup.Key.StaticHelpings
                .Select(h => h * mealCount)
                .ToList();
            
            var scaledFoodGrouping = new FoodGrouping(
                mealGroup.Key.Name,
                scaledStaticHelpings,
                mealGroup.Key.PFood,
                mealGroup.Key.FFood,
                mealGroup.Key.CFood,
                mealGroup.Key.PreparationMethod);
            
            return new Meal(
                $"{mealGroup.Key.Name} - {mealCount} meals",
                totalMacros,
                scaledFoodGrouping);
        });
        return summedMeals;
    }
}
