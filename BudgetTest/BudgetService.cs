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

        var period = new Period(start, end);

        return _budgetRepo.GetAll().Sum(budget => budget.OverlappingAmount(period));
    }
}