using Spectre.Console;
using SystemOfEquations.Data;
using SystemOfEquations.Extensions;
using SystemOfEquations.Todoist;

namespace SystemOfEquations;

internal static class InteractiveNavigator
{
    public static async Task RunAsync(Phase phase)
    {
        var running = true;
        while (running)
        {
            running = await ShowMainMenuAsync(phase);
        }
    }

    private static async Task<bool> ShowMainMenuAsync(Phase phase)
    {
        AnsiConsole.Clear();

        // Display header
        var panel = new Panel(new Markup($"[bold]{phase.Name}[/]"))
        {
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        // Calculate and display week average
        var (totalActualCals, totalActualMacros, totalActualFiber, totalTargetMacros, hasConversionFoods) = CalculateWeekTotals(phase.TrainingWeek);

        if (hasConversionFoods)
        {
            AnsiConsole.MarkupLine($"[dim]Week Average:[/]");
            AnsiConsole.MarkupLine($"[dim]  ACTUAL: {totalActualCals / 7:F0} cals, {totalActualMacros / 7}, {totalActualFiber / 7:F1}g fiber[/]");
            AnsiConsole.MarkupLine($"[dim]  TARGET: {totalTargetMacros / 7}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[dim]Week Average: {totalActualCals / 7:F0} cals, {totalActualMacros / 7}, {totalActualFiber / 7:F1}g fiber[/]");
        }

        AnsiConsole.WriteLine();

        // Create menu choices
        var choices = new List<string>
        {
            $"CrossFit Day ({phase.TrainingWeek.XFitDay.TrainingDayType.DaysTraining.Count}x/week)",
            $"Running Day ({phase.TrainingWeek.RunningDay.TrainingDayType.DaysTraining.Count}x/week)",
            $"Rest Day ({phase.TrainingWeek.NonworkoutDay.TrainingDayType.DaysTraining.Count}x/week)",
            "[yellow]Sync to Todoist[/]",
            "[red]Exit[/]"
        };

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select an option:[/]")
                .AddChoices(choices)
                .HighlightStyle(new Style(foreground: Color.Blue)));

        if (selection.Contains("CrossFit"))
        {
            await ShowTrainingDayDetailsAsync(phase, phase.TrainingWeek.XFitDay);
            return true;
        }
        else if (selection.Contains("Running"))
        {
            await ShowTrainingDayDetailsAsync(phase, phase.TrainingWeek.RunningDay);
            return true;
        }
        else if (selection.Contains("Rest"))
        {
            await ShowTrainingDayDetailsAsync(phase, phase.TrainingWeek.NonworkoutDay);
            return true;
        }
        else if (selection.Contains("Sync"))
        {
            await SyncToTodoistAsync(phase);
            return true;
        }
        else // Exit
        {
            return false;
        }
    }

    private static async Task ShowTrainingDayDetailsAsync(Phase phase, TrainingDay trainingDay)
    {
        var running = true;
        while (running)
        {
            AnsiConsole.Clear();

            // Display header
            // Check if any meal has conversion foods
            var hasConversionFoods = trainingDay.Meals.Any(m => m.HasConversionFoods);

            string header;
            if (hasConversionFoods)
            {
                // Calculate actual macros (sum of non-conversion servings from all meals)
                // This matches what individual meals display
                var actualMacros = new Macros(0, 0, 0);
                foreach (var meal in trainingDay.Meals)
                {
                    var nonConversionServings = meal.Servings.Where(s => !s.IsConversion);
                    foreach (var serving in nonConversionServings)
                    {
                        actualMacros += serving.NutritionalInformation.Macros;
                    }
                }

                // Sum up target macros from all meals
                var targetMacros = new Macros(0, 0, 0);
                foreach (var meal in trainingDay.Meals)
                {
                    targetMacros += meal.Macros;
                }

                header = $"{trainingDay.TrainingDayType.Name}\nACTUAL: {actualMacros}\nTARGET: {targetMacros}";
            }
            else
            {
                var nutrients = trainingDay.ActualNutrients;
                header = $"{trainingDay.TrainingDayType.Name}\n{nutrients.Macros}";
            }

            var panel = new Panel(new Markup($"[bold]{header}[/]"))
            {
                Border = BoxBorder.Rounded
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            // Get meals for this training day (matches Todoist Eating project structure)
            var meals = trainingDay.Meals.ToList();

            if (meals.Any())
            {
                // Create meal choices with index - matching Todoist format: "1 - Meal Name"
                var mealChoices = meals
                    .Select((m, index) => $"{index + 1} - {m.Name}")
                    .Concat(new[] { "[yellow]Back[/]" })
                    .ToList();

                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select a meal:[/]")
                        .AddChoices(mealChoices)
                        .HighlightStyle(new Style(foreground: Color.Blue)));

                if (selection.Contains("Back"))
                {
                    running = false;
                }
                else
                {
                    // Extract the index from selection (e.g., "1 - Waking" -> index 0)
                    var dashIndex = selection.IndexOf(" - ");
                    if (dashIndex > 0 && int.TryParse(selection.Substring(0, dashIndex), out int mealNumber))
                    {
                        var selectedMeal = meals[mealNumber - 1];
                        await ShowMealDetailsAsync(selectedMeal);
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[dim]No meals found for this day[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to go back...[/]");
                Console.ReadKey(true);
                running = false;
            }
        }
    }

    private static Task ShowMealDetailsAsync(Meal meal)
    {
        AnsiConsole.Clear();

        var panel = new Panel(new Markup($"[bold]{meal.Name}[/]"))
        {
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        DisplayMeal(meal);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to go back...[/]");
        Console.ReadKey(true);

        return Task.CompletedTask;
    }

    private static void DisplayMeal(Meal meal)
    {
        // Show food grouping name if PrepareInAdvance (matches Todoist behavior)
        if (meal.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareInAdvance)
        {
            AnsiConsole.MarkupLine($"[bold]{meal.FoodGrouping.Name}[/]");
            AnsiConsole.WriteLine();
        }

        // Show all servings (except conversions) - this matches the full meal info
        // For PrepareInAdvance, this includes both WithMeal (cooked) and AtEatingTime servings
        var servingsToDisplay = meal.Servings.Where(s => !s.IsConversion);

        var servingsList = servingsToDisplay.ToList();

        // Display servings if any
        if (servingsList.Any())
        {
            AnsiConsole.MarkupLine("[bold]Ingredients:[/]");
            var servings = TodoistServiceHelper.FormatServingsAsStrings(servingsList);

            foreach (var serving in servings)
            {
                AnsiConsole.MarkupLine($"  • {serving}");
            }

            AnsiConsole.WriteLine();
        }

        // Display macros exactly as they appear in Todoist comments
        AnsiConsole.MarkupLine("[bold]Nutrition:[/]");
        var comment = TodoistServiceHelper.GenerateNutritionalComment(
            servingsList,
            meal.Macros,
            meal.HasConversionFoods);

        // Format the comment for display (indent each line)
        var commentLines = comment.Split('\n');
        foreach (var line in commentLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                AnsiConsole.WriteLine();
            }
            else
            {
                AnsiConsole.MarkupLine($"  [dim]{line}[/]");
            }
        }
    }

    private static async Task SyncToTodoistAsync(Phase phase)
    {
        AnsiConsole.Clear();

        var confirm = AnsiConsole.Confirm("[yellow]Sync meal prep plan to Todoist?[/]");

        if (confirm)
        {
            AnsiConsole.WriteLine();
            await AnsiConsole.Status()
                .StartAsync("[yellow]Syncing to Todoist...[/]", async ctx =>
                {
                    await TodoistService.SyncAsync(phase);
                });

            AnsiConsole.MarkupLine("[green]✓ Synced successfully![/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }

    private static (decimal TotalCals, Macros TotalMacros, decimal TotalFiber, Macros TotalTargetMacros, bool HasConversionFoods) CalculateWeekTotals(TrainingWeek trainingWeek)
    {
        var totalCals = 0.0M;
        var totalMacros = new Macros(0, 0, 0);
        var totalFiber = 0.0M;
        var totalTargetMacros = new Macros(0, 0, 0);
        var hasConversionFoods = false;

        foreach (var trainingDay in trainingWeek.TrainingDays)
        {
            var daysPerWeek = trainingDay.TrainingDayType.DaysTraining.Count;
            totalCals += trainingDay.ActualNutrients.Cals * daysPerWeek;
            totalMacros += trainingDay.ActualNutrients.Macros * daysPerWeek;
            totalFiber += trainingDay.ActualNutrients.Fiber * daysPerWeek;
            totalTargetMacros += trainingDay.TargetMacros * daysPerWeek;
            hasConversionFoods |= trainingDay.HasConversionFoods;
        }

        return (totalCals, totalMacros, totalFiber, totalTargetMacros, hasConversionFoods);
    }
}
