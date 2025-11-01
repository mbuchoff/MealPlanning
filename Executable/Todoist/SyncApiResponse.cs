// ABOUTME: Sync API v9 response structures for batch command execution.
// Maps temporary IDs to real IDs and reports command execution status.
// SyncStatus values can be either "ok" (string) or error objects.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemOfEquations.Todoist;

/// <summary>
/// Response from a Sync API v9 batch execution.
/// </summary>
internal record SyncApiResponse(
    [property: JsonPropertyName("sync_status")] Dictionary<string, JsonElement> SyncStatus,
    [property: JsonPropertyName("temp_id_mapping")] Dictionary<string, string> TempIdMapping
)
{
    /// <summary>
    /// Checks if any commands in the batch failed and throws an exception with details.
    /// </summary>
    public void ThrowIfAnyFailed()
    {
        var failures = SyncStatus
            .Where(kvp => kvp.Value.ValueKind == JsonValueKind.Object)
            .Select(kvp => new
            {
                Uuid = kvp.Key,
                Error = kvp.Value.Deserialize<SyncError>()
            })
            .Where(x => x.Error != null)
            .ToList();

        if (failures.Any())
        {
            var errorMessages = string.Join("\n", failures.Select(f =>
                $"Command {f.Uuid}: {f.Error}"));
            throw new InvalidOperationException($"Batch execution had {failures.Count} failure(s):\n{errorMessages}");
        }
    }
}

/// <summary>
/// Error information from a failed Sync API command.
/// </summary>
internal record SyncError(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_code")] int ErrorCode,
    [property: JsonPropertyName("error_tag")] string? ErrorTag = null,
    [property: JsonPropertyName("http_code")] int? HttpCode = null,
    [property: JsonPropertyName("error_extra")] JsonElement? ErrorExtra = null
)
{
    public override string ToString()
    {
        var details = $"{Error} (code {ErrorCode})";
        if (ErrorTag != null)
            details += $" [{ErrorTag}]";
        if (ErrorExtra != null && ErrorExtra.Value.ValueKind != JsonValueKind.Undefined)
            details += $" - Extra: {ErrorExtra.Value}";
        return details;
    }
};
