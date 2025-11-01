// ABOUTME: Integration tests verifying that batching works correctly with TodoistServiceHelper.
// These tests ensure the refactored batching code works with composite food servings.

using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Todoist;

namespace Test;

public class BatchingIntegrationTests
{
    [Fact]
    public async Task AddServingAsync_WithCompositeFoodServing_ShouldHandleTempIds()
    {
        // Arrange
        var batch = new CommandBatch();
        var progress = new ProgressTracker(100);

        // Create a composite food serving (has parent + components)
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        var composite = CompositeFoodServing.FromComponents("Meal Prep", new[] { chicken, rice });

        var parentTaskId = "parent-task-123";

        // Act & Assert - Should not throw exception
        await TodoistServiceHelper.CreateTodoistSubtasksAsync(
            composite,
            parentTaskId,
            (content, description, dueString, parentId, projectId) =>
            {
                // This simulates what AddServingAsync now does - returns temp ID string
                var tempId = batch.AddItemAddCommand(content, description, projectId, parentId, dueString);
                return Task.FromResult<object>(tempId); // Returns string, not TodoistTask object
            });

        // Verify commands were added
        Assert.True(batch.Count > 0);
    }

    [Fact]
    public async Task AddServingAsync_WithSimpleFoodServing_ShouldWork()
    {
        // Arrange
        var batch = new CommandBatch();
        var progress = new ProgressTracker(100);

        // Simple food serving (no components)
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));

        var parentTaskId = "parent-task-123";

        // Act & Assert - Should not throw exception
        await TodoistServiceHelper.CreateTodoistSubtasksAsync(
            chicken,
            parentTaskId,
            (content, description, dueString, parentId, projectId) =>
            {
                var tempId = batch.AddItemAddCommand(content, description, projectId, parentId, dueString);
                return Task.FromResult<object>(tempId);
            });

        // Verify exactly one command was added
        Assert.Equal(1, batch.Count);
    }

    [Fact]
    public async Task AddServingAsync_WithNestedComposite_ShouldCreateHierarchy()
    {
        // Arrange
        var batch = new CommandBatch();
        var progress = new ProgressTracker(100);

        // Create nested composite
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        var innerComposite = CompositeFoodServing.FromComponents("Base", new[] { chicken, rice });

        var veggies = new FoodServing("Veggies",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 50, P: 2, F: 0.5M, CTotal: 10, CFiber: 3));
        var outerComposite = CompositeFoodServing.FromComponents("Full Meal", new FoodServing[] { innerComposite, veggies });

        var parentTaskId = "parent-task-123";

        // Act
        await TodoistServiceHelper.CreateTodoistSubtasksAsync(
            outerComposite,
            parentTaskId,
            (content, description, dueString, parentId, projectId) =>
            {
                var tempId = batch.AddItemAddCommand(content, description, projectId, parentId, dueString);
                return Task.FromResult<object>(tempId);
            });

        // Assert - Should create: Full Meal parent + Base parent + Chicken + Rice + Veggies = 5 tasks
        Assert.Equal(5, batch.Count);
    }
}
