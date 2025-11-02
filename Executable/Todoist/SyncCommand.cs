// ABOUTME: Sync API v9 command structures for batching Todoist operations.
// Supports item_add, item_update, item_delete, and note_add commands with temporary ID references.

using System.Text.Json.Serialization;

namespace SystemOfEquations.Todoist;

/// <summary>
/// A single command in a Sync API v9 batch request.
/// </summary>
internal record SyncCommand(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("uuid")] string Uuid,
    [property: JsonPropertyName("args")] object Args,
    [property: JsonPropertyName("temp_id")] string? TempId = null
);

/// <summary>
/// Arguments for item_add command (creating tasks).
/// </summary>
internal record ItemAddArgs(
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("project_id")] string? ProjectId = null,
    [property: JsonPropertyName("parent_id")] string? ParentId = null,
    [property: JsonPropertyName("due_string")] string? DueString = null,
    [property: JsonPropertyName("collapsed")] bool? Collapsed = null,
    [property: JsonPropertyName("child_order")] int? ChildOrder = null
);

/// <summary>
/// Arguments for item_update command (updating task properties like collapsed state).
/// </summary>
internal record ItemUpdateArgs(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("collapsed")] bool? Collapsed = null,
    [property: JsonPropertyName("content")] string? Content = null,
    [property: JsonPropertyName("description")] string? Description = null
);

/// <summary>
/// Arguments for item_delete command (deleting tasks).
/// </summary>
internal record ItemDeleteArgs(
    [property: JsonPropertyName("id")] string Id
);

/// <summary>
/// Arguments for note_add command (adding comments to tasks).
/// </summary>
internal record NoteAddArgs(
    [property: JsonPropertyName("item_id")] string ItemId,
    [property: JsonPropertyName("content")] string Content
);
