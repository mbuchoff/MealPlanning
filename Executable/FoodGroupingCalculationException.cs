namespace SystemOfEquations;

internal sealed class FoodGroupingCalculationException : Exception
{
    public FoodGroupingCalculationException(string message) : base(message)
    {
    }

    public FoodGroupingCalculationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
