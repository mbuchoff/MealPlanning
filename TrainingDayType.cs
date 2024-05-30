﻿namespace SystemOfEquations;

internal record TrainingDayType
{
    public TrainingDayType(string name, IList<Day> daysTraining, int? daysEatingPreparedMeals = null)
    {
        Name = name;
        DaysTraining = daysTraining;
        DaysMealPrepping = daysEatingPreparedMeals ?? daysTraining.Count;
    }

    public string Name { get; }
    public IList<Day> DaysTraining { get; }
    public int DaysMealPrepping { get; }

    public override string ToString() => Name;
}
