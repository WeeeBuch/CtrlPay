using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CtrlPay.Repos.ToDoRepo;

namespace CtrlPay.Avalonia.ViewModels;

public partial class DebtItemViewModel : ObservableObject
{
    [ObservableProperty] private string _description;
    [ObservableProperty] private decimal _amount;

    [ObservableProperty] private string creditPayString;
    [ObservableProperty] private decimal creditAmount;
    [ObservableProperty] private DateTime timestamp;

    [ObservableProperty] private bool isExpanded;
    [RelayCommand] private void ToggleExpand() => IsExpanded = !IsExpanded;

    public FrontendTransactionDTO TransactionDTOBase;

    public DebtItemViewModel(FrontendTransactionDTO transaction)
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
        string addr = ToDoRepo.GetOneTimeAddress(TransactionDTOBase);

        QrCodeViewModel vm = new(addr, TransactionDTOBase);
        QrCodeView view = new() { DataContext = vm };

        var window = new QrCodeWindow
        {
            Content = view,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        window.Show();
    }

    public StatusEnum Status { get; set; }

    public void UpdateCreditAmount(decimal amount)
    {
        CreditAmount = amount;
        CreditPayString = $"{CreditAmount} / {Amount}";
    }
}

public partial class DebtViewModel : ViewModelBase
{
    public RangeObservableCollection<DebtItemViewModel> Debts { get; } = [];

    [ObservableProperty] private bool payableChecked;
    [ObservableProperty] private SortOption selectedSortOrder;
    [ObservableProperty] private List<SortOption> sortOptions;

    public DebtViewModel()
    {
        SortOptions =
        [
            new ("AmountAsc", "DebtView.SortOption.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOption.AmountDesc"),
            new ("DateAsc", "DebtView.SortOption.DateAsc"),
            new ("DateDesc", "DebtView.SortOption.DateDesc")
        ];

        SelectedSortOrder = SortOptions[0];

        ApplySorting(SelectedSortOrder.Key);

        UpdateHandler.CreditAvailableUpdateActions.Add(OnCreditChanged);
        UpdateHandler.NewTransactionAddedActions.Add(TransactionsUpdated);
    }

    public void ApplySorting(string? sortingMethod)
    {
        var resultList = new List<DebtItemViewModel>();

        foreach (var dto in PaymentRepo.GetSortedDebts(sortingMethod, PayableChecked))
        {
            var existingVm = Debts.FirstOrDefault(vm =>
                vm.TransactionDTOBase == dto);

            if (existingVm != null)
            {
                resultList.Add(existingVm);
            }
            else
            {
                resultList.Add(new(dto));
            }
        }
                
        Debts.ReplaceAll(resultList);
    }

    public void OnCreditChanged(decimal amount) => ApplySorting(null);
    partial void OnPayableCheckedChanged(bool value) => ApplySorting(null);
    public void TransactionsUpdated() => ApplySorting(null);
    partial void OnSelectedSortOrderChanged(SortOption value) => ApplySorting(value.Key);
}



