using SystemOfEquations;
using SystemOfEquations.Data;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Test;

public class CompositeFoodServingTests
{
    private readonly FoodServing _yeast;
    private readonly FoodServing _gluten;
    private readonly CompositeFoodServing _composite;

    public CompositeFoodServingTests()
    {
        _yeast = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        _gluten = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        var waterComponent = CreateWaterServing(0.00916M);
        _composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten, waterComponent]);
    }

    [Fact]
    public void CompositeFoodServing_ToString_OutputsComponentsOnSeparateLines()
    {
        // Act
        var output = _composite.ToString();

        // Assert
        Assert.Contains("4 grams nutritional yeast", output);
        Assert.Contains("16 grams gluten", output);
        Assert.Contains("water", output);
        var lines = output.Split('\n');
        Assert.Equal(3, lines.Length); // Should output exactly 3 lines for 3 components
    }

    [Fact]
    public void CompositeFoodServing_Multiplication_ScalesComponents()
    {
        // Act
        var scaled = _composite * 2;
        var output = scaled.ToString();

        // Assert
        Assert.Contains("8 grams nutritional yeast", output);
        Assert.Contains("32 grams gluten", output);
    }

    [Fact]
    public void CompositeFoodServing_Multiplication_ScalesNutrition()
    {
        // Act
        var scaled = _composite * 2;

        // Assert
        Assert.Equal(158, scaled.NutritionalInformation.Cals);
        Assert.Equal(27.04M, scaled.NutritionalInformation.P);
        Assert.Equal(1.31M, scaled.NutritionalInformation.F);
        Assert.Equal(6.76M, scaled.NutritionalInformation.CTotal);
        Assert.Equal(1.5M, scaled.NutritionalInformation.CFiber);
    }

    [Fact]
    public void CompositeFoodServing_Water_IncludedAsComponent()
    {
        // Assert that water is included as a component
        var components = _composite.GetComponentsForDisplay().ToList();
        var waterComponent = components.FirstOrDefault(c => c.Name == "water");

        Assert.NotNull(waterComponent);
        Assert.Equal(0.00916M, waterComponent.NutritionalInformation.ServingUnits);
    }

    [Fact]
    public void CompositeFoodServing_WithScaledServingUnits_OutputsCorrectly()
    {
        // Arrange - scale the composite by 6.7x
        var scaledComposite = _composite * 6.7M;

        // Act
        var output = scaledComposite.ToString();

        // Assert - should output 6.7x scaled components (27 and 107 due to rounding)
        Assert.Contains("27 grams nutritional yeast", output);
        Assert.Contains("107 grams gluten", output);
        Assert.Contains("water", output);
    }

    [Fact]
    public void FromComponents_CalculatesNutritionAutomatically()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        // Act
        var waterComponent = CreateWaterServing(0.00916M);
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent, waterComponent]);

        // Assert - nutrition should be sum of components
        Assert.Equal(79, composite.NutritionalInformation.Cals);  // 15 + 64
        Assert.Equal(13.52M, composite.NutritionalInformation.P);  // 1.25 + 12.27
        Assert.Equal(0.655M, composite.NutritionalInformation.F);  // 0.125 + 0.53
        Assert.Equal(3.38M, composite.NutritionalInformation.CTotal);  // 1.25 + 2.13
        Assert.Equal(0.75M, composite.NutritionalInformation.CFiber);  // 0.75 + 0
        Assert.Equal(1, composite.NutritionalInformation.ServingUnits);
        Assert.Equal(ServingUnits.None, composite.NutritionalInformation.ServingUnit);
    }

    [Fact]
    public void FromComponents_PreservesComponentsAndWater()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));
        var waterComponent = CreateWaterServing(0.00916M);

        // Act
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent, waterComponent]);

        // Assert
        Assert.Equal(3, composite.Components.Count);
        Assert.Equal(yeastComponent, composite.Components[0]);
        Assert.Equal(glutenComponent, composite.Components[1]);
        Assert.Equal(waterComponent, composite.Components[2]);
        Assert.Equal("Auto-calculated Seitan", composite.Name);
    }

    [Fact]
    public void FromComponents_ToString_OutputsComponentsCorrectly()
    {
        // Arrange
        var yeastComponent = new FoodServing("nutritional yeast",
            new NutritionalInformation(4, ServingUnits.Gram, 15, 1.25M, 0.125M, 1.25M, 0.75M));
        var glutenComponent = new FoodServing("gluten",
            new NutritionalInformation(16, ServingUnits.Gram, 64, 12.27M, 0.53M, 2.13M, 0));

        // Act
        var composite = CompositeFoodServing.FromComponents(
            "Auto-calculated Seitan",
            [yeastComponent, glutenComponent]);
        var output = composite.ToString();

        // Assert
        Assert.Contains("4 grams nutritional yeast", output);
        Assert.Contains("16 grams gluten", output);
    }

    [Fact]
    public void GetComponentsForDisplay_WithWater_IncludesWaterAsComponent()
    {
        // Arrange
        var waterComponent = CreateWaterServing(0.0366666666666667M);
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten, waterComponent]);

        // Act
        var components = composite.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(3, components.Count); // yeast, gluten, and water
        Assert.Contains(components, c => c.Name == "nutritional yeast");
        Assert.Contains(components, c => c.Name == "gluten");
        Assert.Contains(components, c => c.Name.Contains("water"));

        // Verify water component has correct amount
        var waterComp = components.First(c => c.Name.Contains("water"));
        Assert.Equal("water", waterComp.Name);
    }

    [Fact]
    public void GetComponentsForDisplay_WithWater_ScalesWaterCorrectly()
    {
        // Arrange
        var waterComponent = CreateWaterServing(0.0366666666666667M);
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten, waterComponent]);

        // Act - scale by 4x
        var scaled = composite * 4;

        // Debug: Check the scaled object's water component
        var waterComponents = scaled.GetComponentsForDisplay().Where(c => c.Name == "water").ToList();
        Assert.Single(waterComponents);
        Assert.Equal(0.1466666666666668M, waterComponents[0].NutritionalInformation.ServingUnits, 10);
        Assert.Equal(4M, scaled.NutritionalInformation.ServingUnits);

        var components = scaled.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(3, components.Count);

        // Verify water component has scaled amount (0.0366666666666667 * 4 ≈ 0.147, rounds to 0.1)
        var scaledWaterComponent = components.First(c => c.Name.Contains("water"));
        // Water component name should just be "water", the amount is in ServingUnits
        Assert.Equal("water", scaledWaterComponent.Name);
    }

    [Fact]
    public void GetComponentsForDisplay_NoWater_DoesNotIncludeWaterComponent()
    {
        // Arrange
        var composite = CompositeFoodServing.FromComponents(
            "Seitan",
            [_yeast, _gluten]);

        // Act
        var components = composite.GetComponentsForDisplay().ToList();

        // Assert
        Assert.Equal(2, components.Count); // only yeast and gluten, no water
        Assert.DoesNotContain(components, c => c.Name.Contains("water"));
    }

    [Fact]
    public async Task CreateTodoistSubtasksAsync_CompositeFoodServing_CreatesHierarchicalStructure()
    {
        // Arrange
        var createdTasks = new List<(string content, string? parentId)>();
        var taskIdCounter = 0;

        // Mock function that captures task creation calls
        async Task<object> mockAddTask(string content, string? description, string? dueString, string? parentId, string? projectId)
        {
            var taskId = $"task_{++taskIdCounter}";
            createdTasks.Add((content, parentId));

            // Return anonymous object with Id property to simulate TodoistTask
            await Task.CompletedTask;
            return new { Id = taskId };
        }

        // Act
        await _composite.CreateTodoistSubtasksAsync("parent_task_id", mockAddTask);

        // Assert
        Assert.Equal(4, createdTasks.Count); // Parent seitan task + 2 component tasks + water

        // First task should be the composite (Seitan) with parent_task_id as parent
        Assert.Equal("Seitan", createdTasks[0].content);
        Assert.Equal("parent_task_id", createdTasks[0].parentId);

        // Second task should be yeast as subtask of Seitan
        Assert.Equal("4 grams nutritional yeast", createdTasks[1].content);
        Assert.Equal("task_1", createdTasks[1].parentId); // Should be child of first created task

        // Third task should be gluten as subtask of Seitan
        Assert.Equal("16 grams gluten", createdTasks[2].content);
        Assert.Equal("task_1", createdTasks[2].parentId); // Should be child of first created task

        // Fourth task should be water as subtask of Seitan
        // The water component now outputs properly formatted
        Assert.Equal("0.01 cups water", createdTasks[3].content);
        Assert.Equal("task_1", createdTasks[3].parentId); // Should be child of first created task
    }

    [Fact]
    public async Task CreateTodoistSubtasksAsync_RegularFoodServing_CreatesSingleTask()
    {
        // Arrange
        var createdTasks = new List<(string content, string? parentId)>();

        async Task<object> mockAddTask(string content, string? description, string? dueString, string? parentId, string? projectId)
        {
            createdTasks.Add((content, parentId));
            await Task.CompletedTask;
            return new { Id = "task_1" };
        }

        // Act
        await _yeast.CreateTodoistSubtasksAsync("parent_task_id", mockAddTask);

        // Assert
        Assert.Single(createdTasks);
        Assert.Equal("4 grams nutritional yeast", createdTasks[0].content);
        Assert.Equal("parent_task_id", createdTasks[0].parentId);
    }

    [Fact]
    public async Task CreateTodoistSubtasksAsync_ScaledComposite_CreatesScaledTasks()
    {
        // Arrange
        var scaledComposite = _composite * 2;
        var createdTasks = new List<(string content, string? parentId)>();
        var taskIdCounter = 0;

        async Task<object> mockAddTask(string content, string? description, string? dueString, string? parentId, string? projectId)
        {
            var taskId = $"task_{++taskIdCounter}";
            createdTasks.Add((content, parentId));
            await Task.CompletedTask;
            return new { Id = taskId };
        }

        // Act
        await scaledComposite.CreateTodoistSubtasksAsync("parent_task_id", mockAddTask);

        // Assert
        Assert.Equal(4, createdTasks.Count); // Parent + 2 components + water

        // Parent task should still be "Seitan" (name doesn't scale)
        Assert.Equal("Seitan", createdTasks[0].content);

        // Components should be scaled
        Assert.Equal("8 grams nutritional yeast", createdTasks[1].content);
        Assert.Equal("32 grams gluten", createdTasks[2].content);
        // Water component now outputs properly formatted
        Assert.Equal("0.02 cups water", createdTasks[3].content);
    }

    // Helper method to create water components
    private static FoodServing CreateWaterServing(decimal cups)
    {
        return Foods.Water_1_Cup with
        {
            NutritionalInformation = Foods.Water_1_Cup.NutritionalInformation with { ServingUnits = cups }
        };
    }
}
