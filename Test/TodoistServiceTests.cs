using SystemOfEquations;
using SystemOfEquations.Data;
using SystemOfEquations.Todoist;

namespace Test;

public class TodoistServiceTests
{
    [Fact]
    public void GenerateNutritionalComment_Should_Include_Total_Nutritional_Information()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));

        var servings = new List<FoodServing> { chicken * 2, rice * 1.5M };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // Total should be: (165*2 + 111*1.5) = 496.5 cals
        Assert.Contains("497 cals", comment); // Rounded to F0

        // Total should include sum of all macros
        // P: 31*2 + 2.6*1.5 = 65.9
        // F: 3.6*2 + 0.9*1.5 = 8.55
        // C: 0*2 + 23*1.5 = 34.5
        // Fiber: 0*2 + 1.8*1.5 = 2.7
        Assert.Contains("66 P", comment); // P rounded
        Assert.Contains("9 F", comment); // F rounded
        Assert.Contains("32 C", comment); // C rounded (note: C is CTotal - CFiber = 34.5 - 2.7 = 31.8)
        Assert.Contains("3g fiber", comment); // Fiber rounded
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Include_Individual_Serving_Information()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));

        var servings = new List<FoodServing> { chicken * 2, rice * 1.5M };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // Should include chicken nutritional info
        Assert.Contains("Chicken", comment);
        Assert.Contains("330 cals", comment); // 165 * 2
        Assert.Contains("62 P", comment); // 31 * 2

        // Should include rice nutritional info
        Assert.Contains("Brown Rice", comment);
        Assert.Contains("167 cals", comment); // 111 * 1.5 rounded
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Handle_Scaled_Servings()
    {
        // Arrange
        var baseServing = new FoodServing("Seitan",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 370, P: 75, F: 2, CTotal: 14, CFiber: 0));

        // Simulate scaling for 2 meals out of 5 total meals (scale factor = 2/5 = 0.4)
        decimal scaleFactor = 2M / 5M;
        var scaledServing = baseServing * scaleFactor;
        var servings = new List<FoodServing> { scaledServing };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // Scaled calories: 370 * 0.4 = 148
        Assert.Contains("148 cals", comment);

        // Scaled protein: 75 * 0.4 = 30
        Assert.Contains("30 P", comment);
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Exclude_Conversion_Servings()
    {
        // Arrange
        var realServing = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0),
            IsConversion: false);

        var conversionServing = new FoodServing("Chicken Cups",
            new(ServingUnits: 1, ServingUnits.Cup, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0),
            IsConversion: true);

        var servings = new List<FoodServing> { realServing, conversionServing };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // Should include the real serving
        Assert.Contains("Chicken", comment);

        // Should NOT include the conversion serving by name in individual breakdown
        // (though its nutrition contributes to total)
        var sections = comment.Split("\n\n");

        // Should only have total + 1 individual serving (not 2)
        Assert.Equal(2, sections.Length); // Total + 1 serving
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Work_For_MealPrep_Quantity_Scenarios()
    {
        // Arrange - Simulate a meal prep plan with 5 total meals
        // This tests the scenario used in AddMealQuantitySubtask
        var baseServings = new List<FoodServing>
        {
            new FoodServing("Seitan",
                new(ServingUnits: 500, ServingUnits.Gram, Cals: 1850, P: 375, F: 10, CTotal: 70, CFiber: 0)),
            new FoodServing("Rice",
                new(ServingUnits: 300, ServingUnits.Gram, Cals: 333, P: 7.8M, F: 2.7M, CTotal: 69, CFiber: 5.4M))
        };

        // Scale for "2 meals" out of 5 total (scale factor = 2/5 = 0.4)
        decimal scaleFactor = 2M / 5M;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings);

        // Assert
        // Total calories: (1850 + 333) * 0.4 = 873.2
        Assert.Contains("873 cals", comment);

        // Total protein: (375 + 7.8) * 0.4 = 153.12
        Assert.Contains("153 P", comment);

        // Should include individual servings
        Assert.Contains("Seitan", comment);
        Assert.Contains("Rice", comment);

        // Seitan calories: 1850 * 0.4 = 740
        Assert.Contains("740 cals", comment);
    }

    [Fact]
    public void CountTodoistOperations_FoodServing_Should_Return_One()
    {
        // Arrange
        var serving = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));

        // Act
        var count = TodoistServiceHelper.CountTodoistOperations(serving);

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void CountTodoistOperations_CompositeFoodServing_Should_Count_Parent_And_Components()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        var veggies = new FoodServing("Veggies",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 50, P: 2, F: 0.5M, CTotal: 10, CFiber: 3));

        var composite = CompositeFoodServing.FromComponents("Meal", new[] { chicken, rice, veggies });

        // Act
        var count = TodoistServiceHelper.CountTodoistOperations(composite);

        // Assert
        // Should be 1 (parent) + 3 (components) = 4
        Assert.Equal(4, count);
    }

    [Fact]
    public void CountTodoistOperations_NestedComposite_Should_Count_Recursively()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));

        // Create inner composite with 2 components
        var innerComposite = CompositeFoodServing.FromComponents("Base Meal", new[] { chicken, rice });
        // innerComposite should count as: 1 (parent) + 2 (components) = 3

        var veggies = new FoodServing("Veggies",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 50, P: 2, F: 0.5M, CTotal: 10, CFiber: 3));

        // Create outer composite containing the inner composite and veggies
        var outerComposite = CompositeFoodServing.FromComponents("Full Meal", new FoodServing[] { innerComposite, veggies });

        // Act
        var count = TodoistServiceHelper.CountTodoistOperations(outerComposite);

        // Assert
        // Should be: 1 (outer parent) + 3 (inner composite) + 1 (veggies) = 5
        Assert.Equal(5, count);
    }

    [Fact]
    public void CountTodoistOperations_StaticFoodServing_Should_Delegate_To_Original()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var staticChicken = new StaticFoodServing(chicken);

        // Act
        var count = TodoistServiceHelper.CountTodoistOperations(staticChicken);

        // Assert
        // Should delegate to the original serving, which returns 1
        Assert.Equal(1, count);
    }

    [Fact]
    public void CountTodoistOperations_StaticComposite_Should_Count_Nested_Operations()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));

        var composite = CompositeFoodServing.FromComponents("Meal", new[] { chicken, rice });
        var staticComposite = new StaticFoodServing(composite);

        // Act
        var count = TodoistServiceHelper.CountTodoistOperations(staticComposite);

        // Assert
        // Should delegate to the composite: 1 (parent) + 2 (components) = 3
        Assert.Equal(3, count);
    }
}
