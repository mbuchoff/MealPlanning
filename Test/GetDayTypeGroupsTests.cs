using SystemOfEquations.Data;
using SystemOfEquations.Data.TrainingWeeks;
using SystemOfEquations.Todoist;
using Xunit;

namespace SystemOfEquations.Test;

public class GetDayTypeGroupsTests
{
    [Fact]
    public void ReturnsThreeGroups_OnePerDayType()
    {
        // Arrange
        var trainingWeek = new MuscleGain2().ForTargetCalories(3000);
        var phase = new Phase("Test Phase", trainingWeek);

        // Act
        var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

        // Assert
        Assert.Equal(3, groups.Count);
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Crossfit day");
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Running day");
        Assert.Contains(groups, g => g.TrainingDayType.ToString() == "Non-weight training day");
    }
}
