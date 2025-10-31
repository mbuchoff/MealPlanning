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

    [Fact]
    public void IncludesPrepareAsNeededMeals_WithAllServings()
    {
        // Arrange
        var trainingWeek = new MuscleGain2().ForTargetCalories(3000);
        var phase = new Phase("Test Phase", trainingWeek);

        // Act
        var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

        // Assert - each group should have meals
        Assert.All(groups, g => Assert.NotEmpty(g.Meals));

        // Verify PrepareAsNeeded meals are included
        var xfitGroup = groups.First(g => g.TrainingDayType.ToString() == "Crossfit day");
        var prepareAsNeededMeals = phase.TrainingWeek.XFitDay.Meals
            .Where(m => m.FoodGrouping.PreparationMethod == FoodGrouping.PreparationMethodEnum.PrepareAsNeeded)
            .ToList();

        Assert.True(prepareAsNeededMeals.Count > 0, "Test setup requires PrepareAsNeeded meals");
        Assert.True(xfitGroup.Meals.Count >= prepareAsNeededMeals.Count);
    }

    [Fact]
    public void MealIndices_ResetPerDayType()
    {
        // Arrange
        var trainingWeek = new MuscleGain2().ForTargetCalories(3000);
        var phase = new Phase("Test Phase", trainingWeek);

        // Act
        var groups = TodoistService.GetDayTypeGroupsPublic(phase).ToList();

        // Assert - each group's meals start at index 1
        foreach (var group in groups.Where(g => g.Meals.Any()))
        {
            Assert.Equal(1, group.Meals.First().Index);

            // Verify sequential numbering
            var indices = group.Meals.Select(m => m.Index).ToList();
            Assert.Equal(Enumerable.Range(1, indices.Count), indices);
        }
    }
}
