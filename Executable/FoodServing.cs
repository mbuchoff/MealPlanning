namespace SystemOfEquations;

public record FoodServing(
    string Name,
    NutritionalInformation NutritionalInformation,
    bool IsConversion = false)
{

    public FoodServing Copy(ServingUnit newServingUnit, decimal newServings) =>
        new(Name,
            new(newServings, newServingUnit,
                NutritionalInformation.Cals,
                NutritionalInformation.P,
                NutritionalInformation.F,
                NutritionalInformation.CTotal,
                NutritionalInformation.CFiber));

    public string ToString(decimal quantity) =>
        $"{NutritionalInformation.ServingUnit.ToString(quantity * NutritionalInformation.ServingUnits)} {Name}";

    public FoodServing Convert(ServingUnit newServingUnit)
    {
        var servingUnit = NutritionalInformation.ServingUnit;

        if (servingUnit.UnitConversion.CentralUnit != newServingUnit.UnitConversion.CentralUnit)
        {
            throw new Exception(
                $"Cannot convert {Name} from {servingUnit} to {newServingUnit} because they have a different " +
                $"{nameof(newServingUnit.UnitConversion.CentralUnit)}.");
        }

        var multiplier = newServingUnit.UnitConversion.NumCentralUnitsInThisUnit /
            (NutritionalInformation.ServingUnits * servingUnit.UnitConversion.NumCentralUnitsInThisUnit);

        return new(Name, new(1, newServingUnit,
            NutritionalInformation.Cals * multiplier,
            NutritionalInformation.P * multiplier,
            NutritionalInformation.F * multiplier,
            NutritionalInformation.CTotal * multiplier,
            NutritionalInformation.CFiber * multiplier),
            IsConversion);
    }

    public static FoodServing operator *(FoodServing fs, decimal multiplier) =>
        fs with
        {
            NutritionalInformation = fs.NutritionalInformation * multiplier
        };

    public override string ToString() => ToString(1);

    // Virtual methods for polymorphic display handling
    public virtual IEnumerable<string> ToOutputLines(string prefix = "")
    {
        yield return $"{prefix}{ToString()}";
    }

    public virtual IEnumerable<FoodServing> GetComponentsForDisplay()
    {
        // Base FoodServing returns itself
        yield return this;
    }

    // Virtual method to handle scaling for composite display
    // Base implementation applies the multiplier normally
    public virtual FoodServing ApplyScale(decimal scale)
    {
        return this * scale;
    }

    // Virtual method to handle Todoist task creation
    // Returns the created task ID if a parent task was created, null otherwise
    public virtual async Task<string?> CreateTodoistSubtasksAsync(
        string parentTaskId,
        Func<string, string?, string?, string?, string?, Task<object>> addTaskFunc)
    {
        // Base FoodServing creates a single subtask
        await addTaskFunc(ToString(), null, null, parentTaskId, null);
        return null;
    }

    // Virtual method to count how many Todoist tasks this serving will create
    public virtual int CountTodoistOperations()
    {
        // Base FoodServing creates 1 task
        return 1;
    }
}
