using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Entities;
using CtrlPay.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    [ObservableProperty] private bool isExpanded;
    [RelayCommand] private void ToggleExpand() => IsExpanded = !IsExpanded;

    public DebtItemViewModel(TransactionDTO transaction)
    {
        _description = transaction.Title;
        _amount = transaction.Amount;
        Status = transaction.State;

        UpdateHandler.CreditAvailableUpdateActions.Add(UpdateCreditAmount);
    }

    [RelayCommand]
    private void PayFromCredit() { /* Implementace platby */ }

    [RelayCommand]
    private void GenerateAddress() { /* Implementace generování adresy */ }

    public TransactionStatusEnum Status { get; set; }

    public void UpdateCreditAmount(decimal amount)
    {
        CreditAmount = amount;
        CreditPayString = $"{CreditAmount} / {_amount}";
    }
}

public partial class DebtViewModel : ViewModelBase
{
    public ObservableCollection<DebtItemViewModel> Debts { get; } = new();

    [ObservableProperty]
    private SortOption selectedSortOrder;

    [ObservableProperty]
    private List<SortOption> sortOptions;

    public DebtViewModel()
    {
        // Sample data pro testování
        Debts.Add(new DebtItemViewModel(new() { Amount = 100, Title = "Popisek", State = TransactionStatusEnum.Completed}));
        Debts.Add(new DebtItemViewModel(new() { Amount = 100, Title = "Popisek02", State = TransactionStatusEnum.Failed }));

        SortOptions = new List<SortOption>
        {
            new ("AmountAsc", "DebtView.SortOption.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOption.AmountDesc"),
            new ("DateAsc", "DebtView.SortOption.DateAsc"),
            new ("DateDesc", "DebtView.SortOption.DateDesc")
        };

        SelectedSortOrder = SortOptions[0];
    }
}

public partial class SortOption : ObservableObject
{
    public string Key { get; set; }

    [ObservableProperty]
    private string displayName;

    public string DisplayNameKay;

    public SortOption(string key, string displayNameKey)
    {
        Key = key;
        DisplayNameKay = displayNameKey;

        UpdateDisplayName();

        TranslationManager.LanguageChanged.Add(UpdateDisplayName);
    }

    private void UpdateDisplayName()
    {
        if (!string.IsNullOrEmpty(DisplayNameKay))
            DisplayName = TranslationManager.GetString(DisplayNameKay);
    }
}