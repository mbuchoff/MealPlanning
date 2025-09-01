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
            foreach (var serving in Servings.Where(s => !s.IsConversion))
            {
                sb.AppendLine(serving.ToString());
            }
        }

        return sb.ToString();
    }

    public string Name { get; } = name;
    public FoodGrouping FoodGrouping { get; } = foodGrouping;

    private IEnumerable<FoodServing>? _servings = null;
    public IEnumerable<FoodServing> Servings
    {
        get
        {
            if (_servings != null)
            {
                return _servings;
            }

            var pMacros = FoodGrouping.PFood.NutritionalInformation.Macros;
            var fMacros = FoodGrouping.FFood.NutritionalInformation.Macros;
            var cMacros = FoodGrouping.CFood.NutritionalInformation.Macros;
            var remainingMacros = Macros - FoodGrouping.StaticServings.Sum(s => s.NutritionalInformation.Macros);

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

                _servings = FoodGrouping.StaticServings.Append(
                [
                    FoodGrouping.PFood * pFoodServings,
                    FoodGrouping.FFood * fFoodServings,
                    FoodGrouping.CFood * cFoodServings,
                ]);

                foreach (var serving in Servings)
                {
                    if (serving.NutritionalInformation.ServingUnits < 0 && !serving.IsConversion)
                    {
                        throw new Exception($"{Name} > {serving.NutritionalInformation.ServingUnits:F2} servings in {serving.Name}.");
                    }
                }
            }

            return _servings;
        }
    }

    public Macros Macros { get; } = macros;
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
