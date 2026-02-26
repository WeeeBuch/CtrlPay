using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;

namespace CtrlPay.Repos;

public class AccountantTransactionDTO
{
    public int Id { get; init; }
    public string Title { get; init; } // XMR Transaction ID
    public string CustomerName { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public StatusEnum State { get; init; }
    public TransactionTypeEnum Type { get; init; } // In/Out

    public AccountantTransactionDTO() { }
}
