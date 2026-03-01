using System;

namespace CtrlPay.Repos;

public class AccountantDashboardSummaryDTO
{
    public decimal OverpaidAmount { get; set; }
    public int OverpaidCount { get; set; }
    public decimal OverdueAmount { get; set; }
    public int OverdueCount { get; set; }
    public decimal PartiallyPaidAmount { get; set; }
    public int PartiallyPaidCount { get; set; }
    public decimal WaitingAmount { get; set; }
    public int WaitingCount { get; set; }
}
