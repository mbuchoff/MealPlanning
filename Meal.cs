﻿using System.Text;
using SystemOfEquations.Data;
using SystemOfEquations.Extensions;

namespace SystemOfEquations;

internal class Meal
{
    public Meal(string name, Macros macros, FoodGrouping foodGrouping)
    {
        FoodGrouping = foodGrouping;

        var pMacros = foodGrouping.PFood.NutritionalInformation.Macros;
        var fMacros = foodGrouping.FFood.NutritionalInformation.Macros;
        var cMacros = foodGrouping.CFood.NutritionalInformation.Macros;

        var remainingMacros = macros - foodGrouping.StaticHelpings.Sum(h => h.Macros);
        (var pFoodServings, var fFoodServings, var cFoodServings) = Equation.Solve(
            new(pMacros.P, fMacros.P, cMacros.P, remainingMacros.P),
            new(pMacros.F, fMacros.F, cMacros.F, remainingMacros.F),
            new(pMacros.C, fMacros.C, cMacros.C, remainingMacros.C));
        Name = name;
        Macros = macros;
        Helpings = foodGrouping.StaticHelpings.Append(
        [
            new Helping(foodGrouping.PFood, pFoodServings),
            new Helping(foodGrouping.FFood, fFoodServings),
            new Helping(foodGrouping.CFood, cFoodServings),
        ]);

        foreach (var helping in Helpings)
        {
            if (helping.Servings < 0 && !helping.Food.IsConversion)
            {
                ErrorState = $"{helping.Servings:F2} servings in {helping.Food.Name}.";
            }
        }
    }

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

    public string Name { get; }
    public string? ErrorState { get; }
    public FoodGrouping FoodGrouping { get; }
    public IEnumerable<Helping> Helpings { get; }
    public Macros Macros { get; }
    public NutritionalInformation NutritionalInformation => Helpings
        .Select(h => h.NutritionalInformation)
        .Sum(1, ServingUnits.Meal);

    public Meal CloneWithTweakedMacros(double pPercent, double fPercent, double cPercent) =>
        new(Name, Macros.CloneWithTweakedMacros(pPercent, fPercent, cPercent), FoodGrouping);
}

internal static class MealExtensions
{
    public static IEnumerable<Meal> SumWithSameFoodGrouping(this IEnumerable<Meal> meals, int daysPerWeek)
    {
        var mealGroups = meals.GroupBy(m => m.FoodGrouping);
        var groupedHelpings = mealGroups
            .Select(mealGroup => mealGroup.SelectMany(m => m.Helpings)
            .CombineLikeHelpings());
        var summedMeals = mealGroups.Select(mealGroup => new Meal(
            $"{mealGroup.Key.Name} - {mealGroup.Count() * daysPerWeek} meals",
            mealGroup.Sum(m => m.Macros),
            mealGroup.Key));
        return summedMeals;
    }
}
