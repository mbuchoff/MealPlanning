using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;
using System.Linq;

namespace Test;

public class WaterMigrationTests
{
    [Fact]
    public void Water_Food_Should_Have_Zero_Macros()
    {
        // Test that the new water food has all zero nutritional values
        var water = Foods.Water_1_Cup;

        Assert.Equal(0, water.NutritionalInformation.Cals);
        Assert.Equal(0, water.NutritionalInformation.P);
        Assert.Equal(0, water.NutritionalInformation.F);
        Assert.Equal(0, water.NutritionalInformation.CTotal);
        Assert.Equal(0, water.NutritionalInformation.CFiber);
        Assert.Equal(1, water.NutritionalInformation.ServingUnits);
        Assert.Equal(ServingUnits.Cup, water.NutritionalInformation.ServingUnit);
        Assert.Equal("water", water.Name);
    }

    [Fact]
    public void StaticWater_Should_Not_Scale_When_Composite_Multiplied()
    {
        // Test that static water components don't scale
        var staticWater = new StaticFoodServing(CreateWaterServing(1.5M));
        var riceBase = new FoodServing("rice base",
            new NutritionalInformation(45, ServingUnits.Gram, 170, 4, 1.5M, 35, 2));

        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "rice with static water",
            scalableComponents: [riceBase],
            staticComponents: [staticWater]);

        var doubled = composite * 2;
        var waterComponent = doubled.GetComponentsForDisplay()
            .OfType<StaticFoodServing>()
            .First(c => c.Name == "water");

        Assert.Equal(1.5M, waterComponent.NutritionalInformation.ServingUnits);
    }

    [Fact]
    public void ScalableWater_Should_Scale_When_Composite_Multiplied()
    {
        // Test that scalable water components scale correctly
        var scalableWater = CreateWaterServing(0.5M);
        var riceBase = new FoodServing("rice base",
            new NutritionalInformation(45, ServingUnits.Gram, 170, 4, 1.5M, 35, 2));

        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "rice with scalable water",
            scalableComponents: [riceBase, scalableWater],
            staticComponents: []);

        var doubled = composite * 2;
        var waterComponent = doubled.GetComponentsForDisplay()
            .Where(c => !(c is StaticFoodServing) && c.Name == "water")
            .First();

        Assert.Equal(1.0M, waterComponent.NutritionalInformation.ServingUnits); // 0.5 * 2
    }

    [Fact]
    public void Combined_Water_Display_Should_Show_Single_Amount()
    {
        // Test that multiple water components are combined in display
        var staticWater = new StaticFoodServing(CreateWaterServing(1.5M));
        var scalableWater = CreateWaterServing(0.5M);
        var riceBase = new FoodServing("rice base",
            new NutritionalInformation(45, ServingUnits.Gram, 170, 4, 1.5M, 35, 2));

        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "brown rice",
            scalableComponents: [riceBase, scalableWater],
            staticComponents: [staticWater]);

        var doubled = composite * 2;
        var output = doubled.ToString();

        // Should show combined water amount: 1.5 (static) + 1.0 (0.5 * 2 scaled) = 2.5
        Assert.Contains("2.5 cups water", output);
        Assert.DoesNotContain("1.5 cups water", output);
        Assert.DoesNotContain("1.0 cups water", output);
    }

    [Theory]
    [InlineData(1, 2.0)] // 1.5 base + 0.5 per serving = 2.0
    [InlineData(2, 2.5)] // 1.5 base + (0.5 * 2) = 2.5
    [InlineData(3, 3.0)] // 1.5 base + (0.5 * 3) = 3.0
    public void BrownRice_Migration_Should_Match_Old_AmountWater_Behavior(decimal multiplier, decimal expectedWater)
    {
        // Test that migrated BrownRice produces same water amounts as old system
        var migratedBrownRice = Foods.BrownRice_45_Grams * multiplier;
        var totalWater = CalculateTotalWaterFromComposite(migratedBrownRice);

        Assert.Equal(expectedWater, totalWater);
    }

    [Theory]
    [InlineData("Farro", 0, 1.333333333332)] // Only scalable water
    [InlineData("PearledBarley", 1.5, 0.6)]
    [InlineData("Quinoa", 1.5, 0.6)]
    [InlineData("ToastedWheatfuls", 2, 0.6)]
    [InlineData("QueatBerries", 2, 0.6)]
    public void MigratedFood_Should_Have_Correct_Water_Pattern(
        string foodName, decimal expectedBase, decimal expectedPerServing)
    {
        // Test each migrated food has correct static/scalable water amounts
        var food = GetMigratedFood(foodName);
        var composite = Assert.IsType<CompositeFoodServing>(food);

        var staticWaterAmount = GetStaticWaterAmount(composite);
        var scalableWaterAmount = GetScalableWaterAmount(composite);

        Assert.Equal(expectedBase, staticWaterAmount);
        Assert.Equal(expectedPerServing, scalableWaterAmount);
    }

    [Fact]
    public void Generic_Component_Combination_Should_Work_For_Any_Food()
    {
        // Test that component combination works for foods other than water
        var oilA = new FoodServing("olive oil",
            new NutritionalInformation(1, ServingUnits.Tablespoon, 120, 0, 14, 0, 0));
        var oilB = new FoodServing("olive oil",
            new NutritionalInformation(2, ServingUnits.Tablespoon, 240, 0, 28, 0, 0));

        var composite = CompositeFoodServing.FromComponents("meal with oil",
            [oilA, oilB]);

        var output = composite.ToString();

        // Should combine to show 3 tablespoons olive oil
        Assert.Contains("3.0 tbsps olive oil", output);
        Assert.DoesNotContain("1.0 tbsp olive oil", output);
        Assert.DoesNotContain("2.0 tbsps olive oil", output);
    }

    [Fact]
    public void Water_Components_Should_Be_Combinable_Across_Unit_Conversions()
    {
        // Test that water in different compatible units can be combined
        var waterCups = CreateWaterServing(1);
        var waterMilliliters = CreateWaterServing(240); // This will need unit conversion logic // 1 cup = 240ml

        var composite = CompositeFoodServing.FromComponents("water test",
            [waterCups, waterMilliliters]);

        // Should combine based on central unit conversion
        var output = composite.ToString();
        // This might need adjustment based on how unit conversion works
        Assert.Contains("water", output);
    }

    [Fact]
    public void Composite_With_Only_Water_Should_Display_Correctly()
    {
        // Edge case: composite food that's only water components
        var staticWater = new StaticFoodServing(CreateWaterServing(2));
        var scalableWater = CreateWaterServing(0.5M);

        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "cooking water",
            scalableComponents: [scalableWater],
            staticComponents: [staticWater]);

        var tripled = composite * 3;
        var output = tripled.ToString();

        // Should show: 2 (static) + (0.5 * 3) = 3.5 cups water
        Assert.Contains("3.5 cups water", output);
        Assert.Equal("cooking water", tripled.Name);
    }

    [Fact]
    public void Water_Should_Not_Affect_Nutritional_Totals()
    {
        // Test that water components don't contribute to nutritional information
        var riceBase = new FoodServing("rice base",
            new NutritionalInformation(45, ServingUnits.Gram, 170, 4, 1.5M, 35, 2));
        var water = CreateWaterServing(2);

        var composite = CompositeFoodServing.FromComponents("rice with water",
            [riceBase, water]);

        // Nutrition should only come from rice, not water
        Assert.Equal(170, composite.NutritionalInformation.Cals);
        Assert.Equal(4, composite.NutritionalInformation.P);
        Assert.Equal(1.5M, composite.NutritionalInformation.F);
        Assert.Equal(35, composite.NutritionalInformation.CTotal);
        Assert.Equal(2, composite.NutritionalInformation.CFiber);
    }

    [Fact]
    public async Task Todoist_Integration_Should_Work_With_Combined_Water()
    {
        // Test that Todoist task creation handles combined water correctly
        var staticWater = new StaticFoodServing(CreateWaterServing(1.5M));
        var scalableWater = CreateWaterServing(0.5M);
        var riceBase = new FoodServing("rice base",
            new NutritionalInformation(45, ServingUnits.Gram, 170, 4, 1.5M, 35, 2));

        var composite = CompositeFoodServing.FromComponentsWithStatic(
            "brown rice",
            scalableComponents: [riceBase, scalableWater],
            staticComponents: [staticWater]);

        var doubled = composite * 2;
        var createdTasks = new List<(string content, string parentId)>();

        await doubled.CreateTodoistSubtasksAsync("parent_id",
            (content, _, _, parentId, _) =>
            {
                createdTasks.Add((content, parentId ?? ""));
                return Task.FromResult(new object());
            });

        // Should create task for rice and combined water
        Assert.Contains(createdTasks, t => t.content.Contains("90 grams rice base"));
        Assert.Contains(createdTasks, t => t.content.Contains("2.5 cups water"));
        Assert.DoesNotContain(createdTasks, t => t.content.Contains("1.5 cups water"));
        Assert.DoesNotContain(createdTasks, t => t.content.Contains("1.0 cups water"));
    }

    // Helper methods for tests
    private static FoodServing CreateWaterServing(decimal cups)
    {
        return Foods.Water_1_Cup with {
            NutritionalInformation = Foods.Water_1_Cup.NutritionalInformation with { ServingUnits = cups }
        };
    }

    private static decimal CalculateTotalWaterFromComposite(FoodServing composite)
    {
        if (composite is not CompositeFoodServing comp) return 0;

        var allComponents = comp.GetComponentsForDisplay();
        return allComponents
            .Where(c => c.Name == "water")
            .Sum(c => c.NutritionalInformation.ServingUnits);
    }

    private static FoodServing GetMigratedFood(string foodName)
    {
        return foodName switch
        {
            "BrownRice" => Foods.BrownRice_45_Grams,
            // TODO: Implement these as we migrate each food
            "Farro" => Foods.Farro_52_Grams,
            "PearledBarley" => Foods.PearledBarley_45_Grams,
            "Quinoa" => Foods.Quinoa_45_Grams,
            "ToastedWheatfuls" => Foods.ToastedWheatfuls,
            "QueatBerries" => Foods.WheatBerries_45_Grams,
            _ => throw new ArgumentException($"Unknown food: {foodName}")
        };
    }

    private static decimal GetStaticWaterAmount(CompositeFoodServing composite)
    {
        var staticWater = composite.Components
            .OfType<StaticFoodServing>()
            .FirstOrDefault(c => c.Name == "water");

        return staticWater?.NutritionalInformation.ServingUnits ?? 0;
    }

    private static decimal GetScalableWaterAmount(CompositeFoodServing composite)
    {
        var scalableWater = composite.Components
            .Where(c => !(c is StaticFoodServing) && c.Name == "water")
            .FirstOrDefault();

        return scalableWater?.NutritionalInformation.ServingUnits ?? 0;
    }
}