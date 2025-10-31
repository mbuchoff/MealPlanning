using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Todoist;
using Xunit;

namespace SystemOfEquations.Test;

public class CalculateTotalOperationsTests
{
    [Fact]
    public void IncludesDayTypeParentOperations()
    {
        // Arrange
        var trainingWeek = new MuscleGain2().ForTargetCalories(3000);
        var phase = new Phase("Test Phase", trainingWeek);

        // Act
        var totalOps = TodoistService.CalculateTotalOperationsPublic(phase, 0, 0);

        // Assert - should include operations for 3 day-type parents (create + collapse each)
        // This is a smoke test - just verify it doesn't crash and returns reasonable number
        Assert.True(totalOps > 6, "Should include at least 6 ops for day-type parents (3 * 2)");
    }
}
