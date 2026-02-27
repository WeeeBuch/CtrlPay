using System;
using System.Collections.Generic;

namespace CtrlPay.Repos;

public class IncomeChartPointDTO
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}

public class AccountantChartDataDTO
{
    public List<IncomeChartPointDTO> IncomeHistory { get; set; } = new();
    public List<(string Status, int Count)> StatusBreakdown { get; set; } = new();
}
