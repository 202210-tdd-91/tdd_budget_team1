#region

using System;

#endregion

namespace BudgetTest;

public class Period
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    private DateTime End { get; }
    private DateTime Start { get; }

    public int OverlappingDays(Budget budget)
    {
        var overlappingStart = Start > budget.GetFirstDay()
            ? Start
            : budget.GetFirstDay();
        var overlappingEnd = End < budget.GetLastDay()
            ? End
            : budget.GetLastDay();

        return (overlappingEnd - overlappingStart).Days + 1;
    }
}