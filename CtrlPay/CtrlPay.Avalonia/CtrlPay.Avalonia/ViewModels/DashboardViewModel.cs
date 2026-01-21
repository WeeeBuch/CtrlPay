using Avalonia.Threading;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        //TODO: Rozhodit tyhle věci do vlastních threadů, ps: všechny s errorem CS4014
        LoadTransactionLists();
        LoadTransactionSums();
    }

    private async void LoadTransactionSums()
    {
        decimal creditAmount = TransactionRepo.GetTransactionSum();
        decimal pendingAmount = PaymentRepo.GetPaymentSum();

        TotalCredits.Amount = creditAmount;
        PendingCredits.Amount = pendingAmount;

        TotalCredits.GiveTitleKey("CounterPiece.Credits.Title");
        PendingCredits.GiveTitleKey("CounterPiece.Pending.Title");

        UpdateHandler.CreditAvailableUpdateActions.Add((newAmount) =>
        {
            TotalCredits.Amount = newAmount;
        });

        UpdateHandler.PendingPaymentsUpdateActions.Add((newAmount) =>
        {
            PendingCredits.Amount = newAmount;
        });
    }

    private async Task LoadTransactionLists()
    {
        CancellationToken cancellationToken = new CancellationToken();
        List<FrontendTransactionDTO> creditTransactions = TransactionRepo.GetTransactions();
        List<FrontendTransactionDTO> pendingTransactions = PaymentRepo.GetPayments();

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

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            CreditTransactionList.RefreshTransactions(creditData);
            PendingTransactionList.RefreshTransactions(pendingData);
        });
    }
}