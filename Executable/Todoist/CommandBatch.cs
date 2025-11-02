// ABOUTME: Accumulates Sync API v9 commands for batch execution with automatic splitting at 100 commands.
// Manages temporary ID generation and mapping to real IDs after execution.

namespace SystemOfEquations.Todoist;

/// <summary>
/// Accumulates Sync API commands for batch execution.
/// Auto-flushes when reaching 100 commands (Todoist API limit).
/// Tracks temporary ID mappings from executed batches.
/// </summary>
internal class CommandBatch
{
    private readonly List<SyncCommand> _commands = new();
    private readonly Dictionary<string, string> _tempIdToRealId = new();
    private int _tempIdCounter = 0;

    /// <summary>
    /// Maximum number of commands per batch allowed by Todoist API.
    /// </summary>
    public const int MaxCommandsPerBatch = 100;

    /// <summary>
    /// Number of commands currently in the batch.
    /// </summary>
    public int Count => _commands.Count;

    /// <summary>
    /// Whether the batch is at capacity and needs to be flushed.
    /// </summary>
    public bool IsFull => _commands.Count >= MaxCommandsPerBatch;

    /// <summary>
    /// Whether the batch has any commands to execute.
    /// </summary>
    public bool IsEmpty => _commands.Count == 0;

    /// <summary>
    /// All commands currently in the batch.
    /// </summary>
    public IReadOnlyList<SyncCommand> Commands => _commands.AsReadOnly();

    /// <summary>
    /// Adds an item_add command to the batch.
    /// Returns a temporary ID that will be mapped to the real ID after execution.
    /// </summary>
    public string AddItemAddCommand(
        string content,
        string? description = null,
        string? projectId = null,
        string? parentId = null,
        string? dueString = null,
        bool? collapsed = null,
        int? childOrder = null)
    {
        var tempId = GenerateTempId();
        var uuid = Guid.NewGuid().ToString();

        // Resolve parent ID if it's a temp ID
        var resolvedParentId = parentId != null ? ResolveTempId(parentId) : null;
        var resolvedProjectId = projectId != null ? ResolveTempId(projectId) : null;

        var args = new ItemAddArgs(
            Content: content,
            Description: description,
            ProjectId: resolvedProjectId,
            ParentId: resolvedParentId,
            DueString: dueString,
            Collapsed: collapsed,
            ChildOrder: childOrder
        );

        _commands.Add(new SyncCommand("item_add", uuid, args, tempId));
        return tempId;
    }

    /// <summary>
    /// Adds an item_update command to the batch.
    /// </summary>
    public void AddItemUpdateCommand(
        string id,
        bool? collapsed = null,
        string? content = null,
        string? description = null)
    {
        var uuid = Guid.NewGuid().ToString();
        var resolvedId = ResolveTempId(id);

        var args = new ItemUpdateArgs(
            Id: resolvedId,
            Collapsed: collapsed,
            Content: content,
            Description: description
        );

        // temp_id is required by Todoist API even for update commands
        _commands.Add(new SyncCommand("item_update", uuid, args, TempId: GenerateTempId()));
    }

    /// <summary>
    /// Adds an item_delete command to the batch.
    /// </summary>
    public void AddItemDeleteCommand(string id)
    {
        var uuid = Guid.NewGuid().ToString();
        var resolvedId = ResolveTempId(id);

        var args = new ItemDeleteArgs(Id: resolvedId);
        // temp_id is required by Todoist API even for delete commands
        _commands.Add(new SyncCommand("item_delete", uuid, args, TempId: GenerateTempId()));
    }

    /// <summary>
    /// Adds a note_add command to the batch.
    /// </summary>
    public void AddNoteAddCommand(string itemId, string content)
    {
        var uuid = Guid.NewGuid().ToString();
        var resolvedItemId = ResolveTempId(itemId);

        var args = new NoteAddArgs(ItemId: resolvedItemId, Content: content);
        // temp_id is required by Todoist API even for note_add commands
        _commands.Add(new SyncCommand("note_add", uuid, args, TempId: GenerateTempId()));
    }

    /// <summary>
    /// Records temp ID mappings from a batch execution response.
    /// </summary>
    public void RecordTempIdMappings(Dictionary<string, string> mappings)
    {
        foreach (var (tempId, realId) in mappings)
        {
            _tempIdToRealId[tempId] = realId;
        }
    }

    /// <summary>
    /// Clears all commands from the batch (after successful execution).
    /// Does NOT clear temp ID mappings - those persist across batches.
    /// </summary>
    public void Clear()
    {
        _commands.Clear();
    }

    /// <summary>
    /// Resolves a temporary ID to its real ID if it's been mapped.
    /// Returns the original ID if it's not a temp ID or hasn't been mapped yet.
    /// </summary>
    private string ResolveTempId(string id)
    {
        return _tempIdToRealId.TryGetValue(id, out var realId) ? realId : id;
    }

    /// <summary>
    /// Generates a unique temporary ID for items being created.
    /// </summary>
    private string GenerateTempId()
    {
        return $"temp-{++_tempIdCounter}";
    }
}
