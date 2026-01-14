using CtrlPay.Avalonia.Translations;
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
    public CounterPieceModel TotalCredits { get; set; } = new();

    public CounterPieceModel PendingCredits { get; set; } = new();

    public TransactionListPieceModel CreditTransactionList { get; } = new();
    public TransactionListPieceModel PendingTransactionList { get; } = new();

    public DashboardViewModel()
    {
        LoadInitialData();
    }

    private void LoadInitialData()
    {
        LoadTransactionLists();
        LoadTransactionSums();
    }

    private void LoadTransactionSums()
    {
        decimal creditAmount = Repos.ToDoRepo.GetTransactionSums("credits");
        decimal pendingAmount = Repos.ToDoRepo.GetTransactionSums("pending");

        CounterPieceModel creditCounter = new()
        {
            Title = TranslationManager.GetString("CounterPiece.Credits.Title"),
            Amount = creditAmount
        };

        CounterPieceModel pendingCounter = new()
        {
            Title = TranslationManager.GetString("CounterPiece.Pending.Title"),
            Amount = pendingAmount
        };

        TotalCredits = creditCounter;
        PendingCredits = pendingCounter;
    }

    private void LoadTransactionLists()
    {
        List<TransactionDTO> creditTransactions = Repos.ToDoRepo.GetTransactions("credits");
        List<TransactionDTO> pendingTransactions = Repos.ToDoRepo.GetTransactions("pending");

        var creditData = creditTransactions.Select(t => new TransactionItemViewModel
        {
            Title = t.Title,
            Amount = t.Amount,
            Date = t.Timestamp,
            Status = t.State
        }).ToList();

        var pendingData = pendingTransactions.Select(t => new TransactionItemViewModel
        {
            Title = t.Title,
            Amount = t.Amount,
            Date = t.Timestamp,
            Status = t.State
        }).ToList();

        CreditTransactionList.RefreshTransactions(creditData);
        PendingTransactionList.RefreshTransactions(pendingData);
    }
}