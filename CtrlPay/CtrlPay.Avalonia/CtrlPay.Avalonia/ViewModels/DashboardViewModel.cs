using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CtrlPay.Repos.ToDoRepo;

namespace CtrlPay.Avalonia.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    // TODO: Zjistit jmeno a upravit popisky
    public CounterPieceModel TotalCredits { get; } = new()
    {
        Title = "Celková suma kreditů",
        Amount = 20.000000000000m
    };

    public CounterPieceModel PendingCredits { get; } = new()
    {
        Title = "Čekající platby",
        Amount = 1.520000000000m
    };

    public TransactionListPieceModel CreditTransactionList { get; } = new();
    public TransactionListPieceModel PendingTransactionList { get; } = new();

    public DashboardViewModel()
    {
        LoadInitialData();
    }

    private void LoadInitialData()
    {
        List<TransactionDTO> creditTransactions = Repos.ToDoRepo.GetTransactions("credits");
        List<TransactionDTO> pendingTransactions = Repos.ToDoRepo.GetTransactions("pending");

        var creditData = creditTransactions.Select(t => new TransactionItemViewModel
        {
            Title = t.Title,
            Amount = t.Amount,
            Date = t.Timestamp
        }).ToList();

        var pendingData = pendingTransactions.Select(t => new TransactionItemViewModel
        {
            Title = t.Title,
            Amount = t.Amount,
            Date = t.Timestamp
        }).ToList();

        CreditTransactionList.RefreshTransactions(creditData);
        PendingTransactionList.RefreshTransactions(pendingData);
    }
}