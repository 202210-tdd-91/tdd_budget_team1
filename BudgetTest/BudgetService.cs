#region

using System;
using System.Linq;

#endregion

namespace BudgetTest;

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
            var middleAmount = 0m;
            var current = start.AddMonths(1);

            var loopStopCondition = new DateTime(end.Year, end.Month, 1);
            while (current < loopStopCondition)
            {
                var budget = GetMonthBudget(current);
                middleAmount += budget.Amount;
                current = current.AddMonths(1);
            }

            var budgetStart = GetMonthBudget(start);
            var startBudgetPerDay = GetBudgetPerDay(start, budgetStart.Amount);
            var dayDiffStart = GetDayDiff(start, new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)));
            var startAmount = CalculateAmount(dayDiffStart, startBudgetPerDay);

            var budgetEnd = GetMonthBudget(end);
            var endBudgetPerDay = GetBudgetPerDay(end, budgetEnd.Amount);
            var dayDiffEnd = GetDayDiff(new DateTime(end.Year, end.Month, 01), end);
            var endAmount = CalculateAmount(dayDiffEnd, endBudgetPerDay);

            return middleAmount + (startAmount + endAmount);
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