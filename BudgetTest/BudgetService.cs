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
            var totalAmount = 0m;
            // var current = start.AddMonths(1);
            var current = start;

            var loopStopCondition = new DateTime(end.Year, end.Month, 1);
            decimal startAmount = 0;
            decimal endAmount = 0;
            while (current < loopStopCondition.AddMonths(1))
            {
                var budget = GetMonthBudget(current);
                var dailyAmount = GetBudgetPerDay(current, budget.Amount);
                if (current.ToString("yyyyMM") == start.ToString("yyyyMM"))
                {
                    var dayDiffStart = GetDayDiff(start, new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)));
                    totalAmount += CalculateAmount(dayDiffStart, dailyAmount);
                }
                else if (current.ToString("yyyyMM") == end.ToString("yyyyMM"))
                {
                    var dayDiffEnd = GetDayDiff(new DateTime(end.Year, end.Month, 01), end);
                    totalAmount += CalculateAmount(dayDiffEnd, dailyAmount);
                }
                else
                {
                    totalAmount += budget.Amount;
                }

                current = current.AddMonths(1);
            }

            return totalAmount + (startAmount + endAmount);
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