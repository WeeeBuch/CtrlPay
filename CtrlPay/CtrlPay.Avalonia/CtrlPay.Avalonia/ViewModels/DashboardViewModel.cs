using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
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

        TotalCredits.HasButton = true;
        TotalCredits.ButtonPress = new RelayCommand(() => AddCredits());
    }

    private void LoadInitialData()
    {
        AppLogger.Info($"Started loading data into dashboard...");
        LoadTransactionLists();
        LoadTransactionSums();
        AppLogger.Info($"Data laoded into dashboard.");
    }

    private void LoadTransactionSums()
    {
        AppLogger.Info($"Loading sums...");
        TotalCredits.Amount = TransactionRepo.GetTransactionSum();
        PendingCredits.Amount = PaymentRepo.GetPaymentSum();

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

        AppLogger.Info($"Sums loaded.");
    }

    private void LoadTransactionLists()
    {
        AppLogger.Info($"Loading Transactions...");
        List<FrontendTransactionDTO> creditTransactions = TransactionRepo.GetSortedTransactions("DateAsc");
        List<FrontendTransactionDTO> pendingTransactions = PaymentRepo.GetSortedDebts("DateAsc", false);

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

        UpdateHandler.NewDebtsAddedActions.Add(() =>
        {
            CreditTransactionList.RefreshTransactions([.. TransactionRepo.GetSortedTransactions(null)
                .Select(t => new TransactionItemViewModel {
                    Title = t.Title,
                    Amount = t.Amount,
                    Date = t.Timestamp,
                    Status = t.State
                    })
                ]);
        });

        UpdateHandler.NewPaymentsAddedActions.Add(() =>
        {
            PendingTransactionList.RefreshTransactions([.. PaymentRepo.GetSortedDebts(null, false)
                .Select(t => new TransactionItemViewModel {
                    Title = t.Title,
                    Amount = t.Amount,
                    Date = t.Timestamp,
                    Status = t.State
                    })
                ]);
        });

        AppLogger.Info($"Transactions loaded.");
    }

    private void AddCredits()
    {
        string addr = ToDoRepo.GetCreditAddress();

        AppLogger.Info($"Preparing QR and QR window for credits...");
        var window = new QrCodeWindow
        {
            Content = new QrCodeView() { DataContext = new QrCodeViewModel(addr) },
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        AppLogger.Info($"Showing QR window...");
        window.Show();
    }
}