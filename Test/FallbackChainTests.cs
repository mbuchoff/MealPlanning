// ABOUTME: Tests for FallbackChain record type to verify construction, validation, and properties
using SystemOfEquations;
using SystemOfEquations.Data;

namespace Test;

public class FallbackChainTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Less_Than_Two_Groupings()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood = new FoodServing("Carb",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));

        var singleGrouping = new FoodGrouping("Single", [], pFood, fFood, cFood,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new FallbackChain(singleGrouping));
        Assert.Contains("at least 2", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Primary_Should_Return_First_Grouping()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var chain = new FallbackChain(grouping1, grouping2);

        // Act & Assert
        Assert.Same(grouping1, chain.Primary);
    }

    [Fact]
    public void Fallbacks_Should_Return_All_Except_First()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));
        var cFood3 = new FoodServing("Carb3",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 80, P: 1, F: 0, CTotal: 20, CFiber: 4));

        var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping3 = new FoodGrouping("Grouping3", [], pFood, fFood, cFood3,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var chain = new FallbackChain(grouping1, grouping2, grouping3);

        // Act & Assert
        Assert.Equal(2, chain.Fallbacks.Length);
        Assert.Same(grouping2, chain.Fallbacks[0]);
        Assert.Same(grouping3, chain.Fallbacks[1]);
    }

    [Fact]
    public void All_Should_Return_Complete_Array()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var chain = new FallbackChain(grouping1, grouping2);

        // Act & Assert
        Assert.Equal(2, chain.All.Length);
        Assert.Same(grouping1, chain.All[0]);
        Assert.Same(grouping2, chain.All[1]);
    }

    [Fact]
    public void Count_Should_Return_Number_Of_Groupings()
    {
        // Arrange
        var pFood = new FoodServing("Protein",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 20, F: 5, CTotal: 0, CFiber: 0));
        var fFood = new FoodServing("Fat",
            new(ServingUnits: 10, ServingUnits.Gram, Cals: 90, P: 0, F: 10, CTotal: 0, CFiber: 0));
        var cFood1 = new FoodServing("Carb1",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 100, P: 2, F: 0, CTotal: 25, CFiber: 2));
        var cFood2 = new FoodServing("Carb2",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 90, P: 1, F: 0, CTotal: 22, CFiber: 3));

        var grouping1 = new FoodGrouping("Grouping1", [], pFood, fFood, cFood1,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);
        var grouping2 = new FoodGrouping("Grouping2", [], pFood, fFood, cFood2,
            FoodGrouping.PreparationMethodEnum.PrepareInAdvance);

        var chain = new FallbackChain(grouping1, grouping2);

        // Act & Assert
        Assert.Equal(2, chain.Count);
    }
}
