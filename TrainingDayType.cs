namespace SystemOfEquations;

internal record TrainingDayType(string Name, int DaysPerWeek, int MealPrepsPerWeek)
{
    public override string ToString() => Name;
}
