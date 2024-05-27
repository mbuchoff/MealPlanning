namespace SystemOfEquations;

internal record TrainingDayType
{
    public TrainingDayType(string name, IList<Day> daysTraining, int? daysMealPrepping = null)
    {
        Name = name;
        DaysTraining = daysTraining;
        DaysMealPrepping = daysMealPrepping ?? daysTraining.Count;
    }

    public string Name { get; }
    public IList<Day> DaysTraining { get; }
    public int DaysMealPrepping { get; }

    public override string ToString() => Name;
}
