// ABOUTME: Tests for parsing Todoist Sync API v9 responses with mixed success/error status values.

using System.Text.Json;
using SystemOfEquations.Todoist;

namespace Test;

public class SyncApiResponseTests
{
    [Fact]
    public void Parse_SuccessResponse_ShouldWork()
    {
        // Arrange
        var json = @"{
            ""full_sync"": true,
            ""sync_status"": {
                ""uuid-1"": ""ok"",
                ""uuid-2"": ""ok""
            },
            ""temp_id_mapping"": {
                ""temp-1"": ""real-id-1"",
                ""temp-2"": ""real-id-2""
            }
        }";

        // Act
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.TempIdMapping.Count);
        Assert.Equal("real-id-1", response.TempIdMapping["temp-1"]);
        Assert.Equal("real-id-2", response.TempIdMapping["temp-2"]);
    }

    [Fact]
    public void Parse_ErrorResponse_ShouldWork()
    {
        // Arrange - This is the actual response format from Todoist when a command fails
        var json = @"{
            ""full_sync"": true,
            ""sync_status"": {
                ""test-error-uuid"": {
                    ""error"": ""Project not found"",
                    ""error_code"": 21,
                    ""error_extra"": {
                        ""project_id"": ""invalid-project-id-123""
                    },
                    ""error_tag"": ""PROJECT_NOT_FOUND"",
                    ""http_code"": 404
                }
            },
            ""temp_id_mapping"": {}
        }";

        // Act & Assert - Should not throw JsonException
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);
        Assert.NotNull(response);
    }

    [Fact]
    public void Parse_MixedResponse_ShouldWork()
    {
        // Arrange - Mix of success and error
        var json = @"{
            ""full_sync"": true,
            ""sync_status"": {
                ""uuid-1"": ""ok"",
                ""uuid-2"": {
                    ""error"": ""Invalid argument"",
                    ""error_code"": 1,
                    ""http_code"": 400
                },
                ""uuid-3"": ""ok""
            },
            ""temp_id_mapping"": {
                ""temp-1"": ""real-id-1"",
                ""temp-3"": ""real-id-3""
            }
        }";

        // Act
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.TempIdMapping.Count);
        Assert.Equal("real-id-1", response.TempIdMapping["temp-1"]);
        Assert.Equal("real-id-3", response.TempIdMapping["temp-3"]);
    }

    [Fact]
    public void ThrowIfAnyFailed_WithSuccessOnly_ShouldNotThrow()
    {
        // Arrange
        var json = @"{
            ""sync_status"": {
                ""uuid-1"": ""ok"",
                ""uuid-2"": ""ok""
            },
            ""temp_id_mapping"": {}
        }";
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);

        // Act & Assert - Should not throw
        response!.ThrowIfAnyFailed();
    }

    [Fact]
    public void ThrowIfAnyFailed_WithError_ShouldThrow()
    {
        // Arrange
        var json = @"{
            ""sync_status"": {
                ""uuid-1"": {
                    ""error"": ""Project not found"",
                    ""error_code"": 21,
                    ""error_tag"": ""PROJECT_NOT_FOUND"",
                    ""http_code"": 404
                }
            },
            ""temp_id_mapping"": {}
        }";
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => response!.ThrowIfAnyFailed());
        Assert.Contains("Project not found", exception.Message);
        Assert.Contains("code 21", exception.Message);
    }

    [Fact]
    public void ThrowIfAnyFailed_WithMultipleErrors_ShouldThrowWithAllDetails()
    {
        // Arrange
        var json = @"{
            ""sync_status"": {
                ""uuid-1"": ""ok"",
                ""uuid-2"": {
                    ""error"": ""Invalid argument"",
                    ""error_code"": 1,
                    ""http_code"": 400
                },
                ""uuid-3"": {
                    ""error"": ""Project not found"",
                    ""error_code"": 21,
                    ""http_code"": 404
                }
            },
            ""temp_id_mapping"": {}
        }";
        var response = JsonSerializer.Deserialize<SyncApiResponse>(json);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => response!.ThrowIfAnyFailed());
        Assert.Contains("2 failure(s)", exception.Message);
        Assert.Contains("Invalid argument", exception.Message);
        Assert.Contains("Project not found", exception.Message);
    }
}
