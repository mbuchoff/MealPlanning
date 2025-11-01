// ABOUTME: Tests for CommandBatch class to verify batching behavior, temp ID management, and 100-command limit.

using SystemOfEquations.Todoist;

namespace Test;

public class CommandBatchTests
{
    [Fact]
    public void NewBatch_Should_BeEmpty()
    {
        // Arrange & Act
        var batch = new CommandBatch();

        // Assert
        Assert.True(batch.IsEmpty);
        Assert.Equal(0, batch.Count);
        Assert.False(batch.IsFull);
    }

    [Fact]
    public void AddItemAddCommand_Should_ReturnTempId()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        var tempId = batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");

        // Assert
        Assert.NotNull(tempId);
        Assert.StartsWith("temp-", tempId);
        Assert.Equal(1, batch.Count);
        Assert.False(batch.IsEmpty);
    }

    [Fact]
    public void AddItemAddCommand_Should_GenerateUniqueTempIds()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        var tempId1 = batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");
        var tempId2 = batch.AddItemAddCommand("Task 2", description: null, projectId: "project-123");
        var tempId3 = batch.AddItemAddCommand("Task 3", description: null, projectId: "project-123");

        // Assert
        Assert.NotEqual(tempId1, tempId2);
        Assert.NotEqual(tempId2, tempId3);
        Assert.NotEqual(tempId1, tempId3);
        Assert.Equal(3, batch.Count);
    }

    [Fact]
    public void AddItemUpdateCommand_Should_IncrementCount()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        batch.AddItemUpdateCommand("task-id-123", collapsed: true);

        // Assert
        Assert.Equal(1, batch.Count);
        Assert.False(batch.IsEmpty);
    }

    [Fact]
    public void AddItemDeleteCommand_Should_IncrementCount()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        batch.AddItemDeleteCommand("task-id-123");

        // Assert
        Assert.Equal(1, batch.Count);
        Assert.False(batch.IsEmpty);
    }

    [Fact]
    public void AddNoteAddCommand_Should_IncrementCount()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        batch.AddNoteAddCommand("task-id-123", "This is a comment");

        // Assert
        Assert.Equal(1, batch.Count);
        Assert.False(batch.IsEmpty);
    }

    [Fact]
    public void Batch_Should_BeFull_At100Commands()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act - Add 99 commands
        for (int i = 0; i < 99; i++)
        {
            batch.AddItemAddCommand($"Task {i}", description: null, projectId: "project-123");
        }

        // Assert - Not full yet
        Assert.False(batch.IsFull);
        Assert.Equal(99, batch.Count);

        // Act - Add 100th command
        batch.AddItemAddCommand("Task 99", description: null, projectId: "project-123");

        // Assert - Now full
        Assert.True(batch.IsFull);
        Assert.Equal(100, batch.Count);
    }

    [Fact]
    public void Clear_Should_EmptyBatch()
    {
        // Arrange
        var batch = new CommandBatch();
        batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");
        batch.AddItemAddCommand("Task 2", description: null, projectId: "project-123");

        // Act
        batch.Clear();

        // Assert
        Assert.True(batch.IsEmpty);
        Assert.Equal(0, batch.Count);
    }

    [Fact]
    public void RecordTempIdMappings_Should_PreserveMappingsAcrossClears()
    {
        // Arrange
        var batch = new CommandBatch();
        var tempId1 = batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");

        // Simulate a batch execution response
        var mappings = new Dictionary<string, string>
        {
            { tempId1, "real-id-abc123" }
        };

        // Act
        batch.RecordTempIdMappings(mappings);
        batch.Clear();

        // Assert - Batch is empty but mappings are preserved
        Assert.True(batch.IsEmpty);

        // Act - Add a new task with parent ID = tempId1 (should resolve to real ID)
        var tempId2 = batch.AddItemAddCommand("Task 2", description: null, projectId: "project-123", parentId: tempId1);

        // Assert - The command should have the real parent ID, not the temp ID
        var command = batch.Commands.Single();
        var args = (ItemAddArgs)command.Args;
        Assert.Equal("real-id-abc123", args.ParentId);
    }

    [Fact]
    public void AddItemAddCommand_Should_ResolveParentTempId()
    {
        // Arrange
        var batch = new CommandBatch();
        var parentTempId = batch.AddItemAddCommand("Parent Task", description: null, projectId: "project-123");

        // Simulate batch execution and temp ID mapping
        batch.RecordTempIdMappings(new Dictionary<string, string>
        {
            { parentTempId, "real-parent-id-123" }
        });
        batch.Clear();

        // Act - Add child task with temp parent ID
        var childTempId = batch.AddItemAddCommand("Child Task", description: null, projectId: null, parentId: parentTempId);

        // Assert - Parent ID should be resolved to real ID
        var command = batch.Commands.Single();
        var args = (ItemAddArgs)command.Args;
        Assert.Equal("real-parent-id-123", args.ParentId);
    }

    [Fact]
    public void Commands_Should_ContainCorrectCommandTypes()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");
        batch.AddItemUpdateCommand("task-id-123", collapsed: true);
        batch.AddItemDeleteCommand("task-id-456");
        batch.AddNoteAddCommand("task-id-789", "Comment text");

        // Assert
        Assert.Equal(4, batch.Commands.Count);
        Assert.Equal("item_add", batch.Commands[0].Type);
        Assert.Equal("item_update", batch.Commands[1].Type);
        Assert.Equal("item_delete", batch.Commands[2].Type);
        Assert.Equal("note_add", batch.Commands[3].Type);
    }

    [Fact]
    public void Commands_Should_HaveUniqueUuids()
    {
        // Arrange
        var batch = new CommandBatch();

        // Act
        batch.AddItemAddCommand("Task 1", description: null, projectId: "project-123");
        batch.AddItemAddCommand("Task 2", description: null, projectId: "project-123");
        batch.AddItemUpdateCommand("task-id-123", collapsed: true);

        // Assert
        var uuids = batch.Commands.Select(c => c.Uuid).ToList();
        Assert.Equal(3, uuids.Count);
        Assert.Equal(3, uuids.Distinct().Count()); // All UUIDs should be unique
    }

    [Fact]
    public void MaxCommandsPerBatch_Should_Be100()
    {
        // Assert
        Assert.Equal(100, CommandBatch.MaxCommandsPerBatch);
    }
}
