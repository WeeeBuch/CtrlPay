using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.HelperClasses;

public partial class TransactionItem : ObservableObject
{
    [ObservableProperty] private string _description;
    [ObservableProperty] private decimal _amount;

    [ObservableProperty] private string creditPayString;
    [ObservableProperty] private decimal creditAmount;
    [ObservableProperty] private DateTime timestamp;

    [ObservableProperty] private bool isExpanded;
    [RelayCommand] private void ToggleExpand() => IsExpanded = !IsExpanded;

    public FrontendTransactionDTO TransactionDTOBase;

    public TransactionItem(FrontendTransactionDTO transaction)
    {
        Description = transaction.Title;
        Amount = transaction.Amount;
        Status = transaction.State;
        Timestamp = transaction.Timestamp;

        TransactionDTOBase = transaction;
        creditPayString = "";

        UpdateCreditAmount(TransactionRepo.GetTransactionSum());
    }

    [RelayCommand]
    private void PayFromCredit() => ToDoRepo.PayFromCredit(TransactionDTOBase);

    [RelayCommand]
    private void GenerateAddress()
    {
        /* Implementace generování adresy */

        AppLogger.Info($"Asking for new onetime address...");
        string addr = ToDoRepo.GetOneTimeAddress(TransactionDTOBase);

        AppLogger.Info($"Preparing QR and QR window...");
        QrCodeViewModel vm = new(addr, TransactionDTOBase);
        QrCodeView view = new() { DataContext = vm };

        var window = new QrCodeWindow
        {
            Content = view,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        AppLogger.Info($"Showing QR window...");
        window.Show();
    }

    public StatusEnum Status { get; set; }

    public void UpdateCreditAmount(decimal amount)
    {
        CreditAmount = amount;
        CreditPayString = $"{CreditAmount} / {Amount}";
    }
}