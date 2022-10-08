#region

using System;
using System.Linq;

#endregion

namespace BudgetTest;

public class Period
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime End { get; private set; }
    public DateTime Start { get; private set; }
}

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var totalAmount = 0m;
            var current = start;

            var loopStopCondition = new DateTime(end.Year, end.Month, 1);
            while (current < loopStopCondition.AddMonths(1))
            {
                var budget = GetMonthBudget(current);
                var overlappingDays = OverlappingDays(new Period(start, end), budget);

                totalAmount += CalculateAmount(overlappingDays, budget.DailyAmount());

                current = current.AddMonths(1);
            }

            return totalAmount;
        }
        else
        {
            var budget = GetMonthBudget(start);
            var budgetPerDay = GetBudgetPerDay(start, budget.Amount);
            var diffDays = GetDayDiff(start, end);
            return CalculateAmount(diffDays, budgetPerDay);
        }
    }

    private static decimal CalculateAmount(int diffDays, int budgetPerDay)
    {
        return diffDays * budgetPerDay * 100 / 100m;
    }

    private static int GetBudgetPerDay(DateTime date, int amount)
    {
        return amount / DateTime.DaysInMonth(date.Year, date.Month);
    }

    private static int GetDayDiff(DateTime start, DateTime end)
    {
        var diffDays = (end - start).Days + 1;
        return diffDays;
    }

    private static int OverlappingDays(Period period, Budget budget)
    {
        var overlappingStart = period.Start > budget.GetFirstDay()
            ? period.Start
            : budget.GetFirstDay();
        var overlappingEnd = period.End < budget.GetLastDay()
            ? period.End
            : budget.GetLastDay();

        return (overlappingEnd - overlappingStart).Days + 1;
    }

    private Budget GetMonthBudget(DateTime date)
    {
        var budgets = _budgetRepo.GetAll();
        var budget = budgets.FirstOrDefault(x => x.YearMonth == $"{date:yyyyMM}");
        if (budget == null)
        {
            return new Budget
                   {
                       YearMonth = date.ToString("yyyyMM"),
                       Amount = 0
                   };
        }

        return budget;
    }
}