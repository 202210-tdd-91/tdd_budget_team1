#region

using System;

#endregion

namespace BudgetTest;

public class Budget
{
    public Budget()
    {
    }

    public Budget(string yearMonth, int amount)
    {
        YearMonth = yearMonth;
        Amount = amount;
    }

    public int Amount { get; set; }
    public string YearMonth { get; set; } = default!;

    public decimal OverlappingAmount(Period period)
    {
        return period.OverlappingDays(CreatePeriod()) * DailyAmount();
    }

    private Period CreatePeriod()
    {
        return new Period(GetFirstDay(), GetLastDay());
    }

    private decimal DailyAmount()
    {
        return Amount / (decimal)GetLastDay().Day;
    }

    private DateTime GetFirstDay()
    {
        return DateTime.ParseExact(YearMonth, "yyyyMM", null);
    }

    private DateTime GetLastDay()
    {
        var firstDay = GetFirstDay();
        var daysInMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
        return new DateTime(firstDay.Year, firstDay.Month, daysInMonth);
    }
}