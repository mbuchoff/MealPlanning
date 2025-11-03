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
    public void GenerateNutritionalComment_Should_Exclude_Conversion_Servings_From_Individual_Breakdown()
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
        var sections = comment.Split("\n\n");

        // Should have ACTUAL + INTENDED + 1 individual serving = 3 sections
        Assert.Equal(3, sections.Length);
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Exclude_Conversion_Servings_From_Macro_Totals()
    {
        // Arrange
        var realServing = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0),
            IsConversion: false);

        // Conversion food that shifts macros: P: -5, C: +5 (Protein to Carb conversion)
        var conversionServing = new FoodServing("Protein to Carb Conversion",
            new(ServingUnits: 5, ServingUnits.Gram, Cals: 0, P: -5, F: 0, CTotal: 5, CFiber: 0),
            IsConversion: true);

        var servings = new List<FoodServing> { realServing, conversionServing };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // ACTUAL macros should ONLY include the real serving, NOT the conversion
        // Real serving: 165 cals, 31 P, 3.6 F, 0 C
        // Conversion should NOT be added: -5 P, +5 C
        // Expected ACTUAL: "ACTUAL:\n165 cals, 31 P (...%), 4 F (...%), 0 C (...%), 0g fiber"

        // Get the first section (ACTUAL) from the comment
        var actualSection = comment.Split("\n\n")[0];

        Assert.Contains("ACTUAL:", actualSection);
        Assert.Contains("165 cals", actualSection);
        Assert.Contains("31 P", actualSection); // Should be 31, not 26 (31-5)
        Assert.Contains("4 F", actualSection);

        // When C is 0, Macros.ToString() shows "0 C (0.0%)"
        Assert.Contains("0 C", actualSection); // Should be 0, not 5
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
    public void GenerateNutritionalComment_Should_Not_Show_Labels_When_No_Conversion_Foods()
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
        // Should NOT contain ACTUAL or INTENDED labels when no conversion foods
        Assert.DoesNotContain("ACTUAL:", comment);
        Assert.DoesNotContain("INTENDED:", comment);

        // Should still have total + 2 individual servings
        var sections = comment.Split("\n\n");
        Assert.Equal(3, sections.Length); // Total + 2 servings
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Include_Intended_Macros_With_Conversion_Foods()
    {
        // Arrange
        var realServing = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0),
            IsConversion: false);

        // Conversion food that shifts macros: P: -5, C: +5 (Protein to Carb conversion)
        var conversionServing = new FoodServing("Protein to Carb Conversion",
            new(ServingUnits: 5, ServingUnits.Gram, Cals: 0, P: -5, F: 0, CTotal: 5, CFiber: 0),
            IsConversion: true);

        var servings = new List<FoodServing> { realServing, conversionServing };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        var sections = comment.Split("\n\n");

        // Should have ACTUAL, INTENDED, and 1 individual serving (3 sections)
        Assert.Equal(3, sections.Length);

        // First section should be ACTUAL with chicken macros only (no conversion)
        var actualSection = sections[0];
        Assert.Contains("ACTUAL:", actualSection);
        Assert.Contains("165 cals", actualSection); // Chicken only
        Assert.Contains("31 P", actualSection); // Chicken P, not 26 (31-5)
        Assert.Contains("0 C", actualSection); // Chicken C, not 5

        // Second section should be INTENDED with chicken + conversion macros
        var intendedSection = sections[1];
        Assert.Contains("INTENDED:", intendedSection);
        Assert.Contains("165 cals", intendedSection); // Cals unchanged (conversion has 0 cals)
        Assert.Contains("26 P", intendedSection); // Chicken P (31) + conversion P (-5) = 26
        Assert.Contains("5 C", intendedSection); // Chicken C (0) + conversion C (5) = 5

        // Third section should be the individual serving (chicken only, no conversion)
        var individualSection = sections[2];
        Assert.Contains("Chicken", individualSection);
        Assert.DoesNotContain("Conversion", individualSection);
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

    [Fact]
    public void GenerateNutritionalComment_Should_Exclude_Zero_Nutrient_Foods_From_Individual_Breakdown()
    {
        // Arrange
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var creatine = new FoodServing("Creatine",
            new(ServingUnits: 5, ServingUnits.Gram, Cals: 0, P: 0, F: 0, CTotal: 0, CFiber: 0));
        var water = new FoodServing("Water",
            new(ServingUnits: 1, ServingUnits.Cup, Cals: 0, P: 0, F: 0, CTotal: 0, CFiber: 0));

        var servings = new List<FoodServing> { chicken, creatine, water };

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings);

        // Assert
        // Total should include chicken's macros only (zero-nutrient foods don't affect totals)
        Assert.Contains("165 cals", comment);
        Assert.Contains("31 P", comment);

        // Chicken should be in the individual breakdown
        Assert.Contains("Chicken", comment);

        // Zero-nutrient foods should NOT be in the individual breakdown
        Assert.DoesNotContain("Creatine", comment);
        Assert.DoesNotContain("Water", comment);

        // Should have 2 sections: total + 1 individual serving (chicken only)
        var sections = comment.Split("\n\n");
        Assert.Equal(2, sections.Length);
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Include_Target_Macros_When_Provided_With_Conversion_Foods()
    {
        // Arrange - Include conversion food to trigger ACTUAL/TARGET display
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        var conversion = new FoodServing("Macro Adjustment",
            new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0),
            IsConversion: true);

        var servings = new List<FoodServing> { chicken * 2, rice * 1.5M, conversion };
        var targetMacros = new Macros(P: 65, F: 10, C: 35);

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings, targetMacros);

        // Assert
        // Should contain ACTUAL label with actual macros (excluding conversion)
        Assert.Contains("ACTUAL:", comment);
        Assert.Contains("497 cals", comment); // Actual total

        // Should contain TARGET label with target macros
        Assert.Contains("TARGET:", comment);
        Assert.Contains("65 P", comment); // Target P
        Assert.Contains("10 F", comment); // Target F
        Assert.Contains("35 C", comment); // Target C
    }

    [Fact]
    public void GenerateNutritionalComment_Should_Not_Show_Calories_In_Target_Line()
    {
        // Arrange - Include conversion food to trigger ACTUAL/TARGET display
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0));
        var rice = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M));
        var conversion = new FoodServing("Macro Adjustment",
            new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -1, F: 0, CTotal: 1, CFiber: 0),
            IsConversion: true);

        var servings = new List<FoodServing> { chicken * 2, rice * 1.5M, conversion };
        var targetMacros = new Macros(P: 65, F: 10, C: 35);

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings, targetMacros);

        // Assert
        // Split into sections to isolate ACTUAL and TARGET
        var sections = comment.Split("\n\n");

        // Find ACTUAL section (should include actual calories)
        var actualSection = sections.First(s => s.Contains("ACTUAL:"));
        Assert.Contains("497 cals", actualSection); // Actual calories should be shown

        // Find TARGET section (should NOT include any calories)
        var targetSection = sections.First(s => s.Contains("TARGET:"));
        Assert.DoesNotContain("cals", targetSection); // TARGET should not show calories
        Assert.Contains("65 P", targetSection); // But should show target macros
        Assert.Contains("10 F", targetSection);
        Assert.Contains("35 C", targetSection);
    }

    [Fact]
    public void GenerateNutritionalComment_Without_TargetMacros_Should_Not_Show_ACTUAL_And_TARGET_Labels()
    {
        // Arrange - Simulate the CURRENT WRONG behavior where targetMacros is not passed
        var baseServings = new List<FoodServing>
        {
            new FoodServing("Seitan",
                new(ServingUnits: 500, ServingUnits.Gram, Cals: 1850, P: 375, F: 10, CTotal: 70, CFiber: 0)),
            new FoodServing("Rice",
                new(ServingUnits: 300, ServingUnits.Gram, Cals: 333, P: 7.8M, F: 2.7M, CTotal: 69, CFiber: 5.4M))
        };

        decimal scaleFactor = 0.2M; // 1 meal out of 5
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();

        // Act - Call without targetMacros (current behavior in TodoistService.AddMealQuantitySubtask)
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings);

        // Assert - Current behavior: no ACTUAL/TARGET labels (just raw macros)
        Assert.DoesNotContain("ACTUAL:", comment);
        Assert.DoesNotContain("TARGET:", comment);
    }

    [Fact]
    public void GenerateNutritionalComment_With_TargetMacros_But_No_Conversion_Foods_Should_Not_Show_Labels()
    {
        // Arrange - NO conversion foods, but target macros provided
        var chicken = new FoodServing("Chicken",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 165, P: 31, F: 3.6M, CTotal: 0, CFiber: 0),
            IsConversion: false);
        var rice = new FoodServing("Brown Rice",
            new(ServingUnits: 100, ServingUnits.Gram, Cals: 111, P: 2.6M, F: 0.9M, CTotal: 23, CFiber: 1.8M),
            IsConversion: false);

        var servings = new List<FoodServing> { chicken * 2, rice * 1.5M };
        var targetMacros = new Macros(P: 65, F: 10, C: 35);

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(servings, targetMacros);

        // Assert - Should NOT show ACTUAL/TARGET labels when no conversion foods
        // This matches console output behavior: only show labels when there's a discrepancy
        Assert.DoesNotContain("ACTUAL:", comment);
        Assert.DoesNotContain("TARGET:", comment);

        // Should still show macros (unlabeled)
        Assert.Contains("497 cals", comment); // Total
        Assert.Contains("66 P", comment);
    }

    [Fact]
    public void GenerateNutritionalComment_For_MealPrep_Quantity_Should_Show_ACTUAL_And_TARGET_Only_With_Conversion_Foods()
    {
        // Arrange - Simulate meal prep plan scenario with targetMacros AND conversion foods
        var totalTargetMacros = new Macros(P: 375, F: 50, C: 175);

        // Base servings with a conversion food
        var baseServings = new List<FoodServing>
        {
            new FoodServing("Seitan",
                new(ServingUnits: 500, ServingUnits.Gram, Cals: 1850, P: 375, F: 10, CTotal: 70, CFiber: 0),
                IsConversion: false),
            new FoodServing("Macro Adjustment",
                new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -5, F: 0, CTotal: 5, CFiber: 0),
                IsConversion: true)
        };

        // We're generating a comment for "1 meal" out of 5 total meals
        decimal scaleFactor = 0.2M; // 1/5
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();
        var scaledTargetMacros = totalTargetMacros * scaleFactor; // 75 P, 10 F, 35 C

        // Act - Call WITH targetMacros AND conversion foods
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings, scaledTargetMacros);

        // Assert - Should show ACTUAL/TARGET labels when conversion foods present
        Assert.Contains("ACTUAL:", comment);
        Assert.Contains("TARGET:", comment);
        Assert.Contains("75 P", comment); // 375 * 0.2 = 75
        Assert.Contains("10 F", comment); // 50 * 0.2 = 10
        Assert.Contains("35 C", comment); // 175 * 0.2 = 35
    }

    [Fact]
    public void GenerateNutritionalComment_When_HasConversionFoods_Flag_Is_True_Should_Show_Labels_Even_If_Servings_Filtered()
    {
        // Arrange - Simulate REAL meal prep scenario
        // Original meal HAD conversion foods (HasConversionFoods=true)
        // But MealPrepPlan.CookingServings has them filtered out
        // This matches what WeeklyMealsPrepPlans.cs does on line 28
        var totalTargetMacros = new Macros(P: 375, F: 50, C: 175);
        var hasConversionFoods = true; // Original meal had conversion foods

        // Base servings WITHOUT conversion foods (filtered out for cooking)
        var baseServings = new List<FoodServing>
        {
            new FoodServing("Seitan",
                new(ServingUnits: 500, ServingUnits.Gram, Cals: 1850, P: 375, F: 10, CTotal: 70, CFiber: 0),
                IsConversion: false),
            new FoodServing("Rice",
                new(ServingUnits: 300, ServingUnits.Gram, Cals: 333, P: 7.8M, F: 2.7M, CTotal: 69, CFiber: 5.4M),
                IsConversion: false)
        };

        decimal scaleFactor = 0.2M;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();
        var scaledTargetMacros = totalTargetMacros * scaleFactor;

        // Act - Pass hasConversionFoods flag to match console behavior
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings, scaledTargetMacros, hasConversionFoods);

        // Assert - SHOULD show ACTUAL/TARGET labels because HasConversionFoods=true
        // This matches console output behavior in WeeklyMealsPrepPlan.ToString:20
        Assert.Contains("ACTUAL:", comment);
        Assert.Contains("TARGET:", comment);
    }

    [Fact]
    public void GenerateNutritionalComment_When_HasConversionFoods_Flag_Is_False_Should_Not_Show_Labels()
    {
        // Arrange - Original meal did NOT have conversion foods
        var totalTargetMacros = new Macros(P: 375, F: 50, C: 175);
        var hasConversionFoods = false;

        var baseServings = new List<FoodServing>
        {
            new FoodServing("Seitan",
                new(ServingUnits: 500, ServingUnits.Gram, Cals: 1850, P: 375, F: 10, CTotal: 70, CFiber: 0)),
            new FoodServing("Rice",
                new(ServingUnits: 300, ServingUnits.Gram, Cals: 333, P: 7.8M, F: 2.7M, CTotal: 69, CFiber: 5.4M))
        };

        decimal scaleFactor = 0.2M;
        var scaledServings = baseServings.Select(s => s * scaleFactor).ToList();
        var scaledTargetMacros = totalTargetMacros * scaleFactor;

        // Act
        var comment = TodoistServiceHelper.GenerateNutritionalComment(scaledServings, scaledTargetMacros, hasConversionFoods);

        // Assert - Should NOT show labels when HasConversionFoods=false
        Assert.DoesNotContain("ACTUAL:", comment);
        Assert.DoesNotContain("TARGET:", comment);
    }

    [Fact]
    public void GenerateNutritionalComment_For_Eating_Meal_With_Conversion_Foods_Should_Show_Labels()
    {
        // Arrange - Simulate eating meal scenario (like "Eating > Non-weight training day > 3 - Bedtime")
        // GetDayTypeGroups filters out conversion foods from servings (line 361, 372)
        // But the original meal HAD conversion foods

        // Original meal servings (with conversion food)
        var allServings = new List<FoodServing>
        {
            new FoodServing("Protein",
                new(ServingUnits: 30, ServingUnits.Gram, Cals: 120, P: 24, F: 2, CTotal: 2, CFiber: 0),
                IsConversion: false),
            new FoodServing("Macro Adjustment",
                new(ServingUnits: 1, ServingUnits.Gram, Cals: 0, P: -2, F: 0, CTotal: 2, CFiber: 0),
                IsConversion: true)
        };

        // Filtered servings (conversion foods removed - what GetDayTypeGroups creates)
        var filteredServings = allServings.Where(s => !s.IsConversion).ToList();
        var targetMacros = new Macros(P: 22, F: 2, C: 4);
        var hasConversionFoods = allServings.Any(s => s.IsConversion); // true

        // Act - Pass hasConversionFoods flag to match console behavior
        var comment = TodoistServiceHelper.GenerateNutritionalComment(filteredServings, targetMacros, hasConversionFoods);

        // Assert - Should show ACTUAL/TARGET labels because original meal had conversion foods
        Assert.Contains("ACTUAL:", comment);
        Assert.Contains("TARGET:", comment);
    }
}
