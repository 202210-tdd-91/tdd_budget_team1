#region

using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

#endregion

namespace BudgetTest;

public class BudgetTest
{
    private readonly IBudgetRepo _budgetRepo;
    private readonly BudgetService _budgetService;

    public BudgetTest()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    [Fact]
    public void All_Month()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new()
                         {
                             YearMonth = "202210",
                             Amount = 3100
                         }
                     });
        var result = _budgetService.Query(new DateTime(2022, 10, 01), new DateTime(2022, 10, 31));
        result.Should().Be(3100m);
    }

    [Fact]
    public void Cross_A_Month()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new()
                         {
                             YearMonth = "202210",
                             Amount = 3100
                         },
                         new(yearMonth: "202211", amount: 300)
                     });
        var result = _budgetService.Query(new DateTime(2022, 10, 31), new DateTime(2022, 11, 5));
        result.Should().Be(150m);
    }

    [Fact]
    public void Cross_Over_Two_Month()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new()
                         {
                             YearMonth = "202210",
                             Amount = 3100
                         },
                         new(yearMonth: "202211", amount: 300),
                         new(yearMonth: "202212", amount: 31),
                     });
        var result = _budgetService.Query(new DateTime(2022, 10, 31), new DateTime(2022, 12, 3));
        result.Should().Be(100m + 300m + 3);
    }

    [Fact]
    public void day_of_start_less_than_day_of_end_when_cross_3_months()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new(yearMonth: "202210", amount: 3100),
                         new(yearMonth: "202211", amount: 300),
                         new(yearMonth: "202212", amount: 31),
                     });
        var result = _budgetService.Query(
            new DateTime(2022, 10, 1),
            new DateTime(2022, 12, 2));

        result.Should().Be(3100 + 300 + 2);
    }

    [Fact]
    public void invalid_period()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new(yearMonth: "202210", amount: 3100),
                     });
        var result = _budgetService.Query(
            new DateTime(2022, 10, 11),
            new DateTime(2022, 10, 2));

        result.Should().Be(0);
    }

    [Fact]
    public void Partial_Month()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new()
                         {
                             YearMonth = "202210",
                             Amount = 3100
                         }
                     });
        var result = _budgetService.Query(new DateTime(2022, 10, 01), new DateTime(2022, 10, 02));
        result.Should().Be(200m);
    }

    [Fact]
    public void period_no_overlap_before_budget()
    {
        GivenBudgets(new List<Budget>()
                     {
                         new(yearMonth: "202210", amount: 3100),
                     });
        var result = _budgetService.Query(
            new DateTime(2022, 8, 2),
            new DateTime(2022, 9, 4));

        result.Should().Be(0);
    }

    private void GivenBudgets(List<Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}