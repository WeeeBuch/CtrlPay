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
    [ObservableProperty] private DateTime timestamp;

    [ObservableProperty] private bool isExpanded;
    [RelayCommand] private void ToggleExpand() => IsExpanded = !IsExpanded;

    public DebtItemViewModel(TransactionDTO transaction)
    {
        _description = transaction.Title;
        _amount = transaction.Amount;
        Status = transaction.State;
        Timestamp = transaction.Timestamp;

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
        GetDebtsFromRepo();

        SortOptions = new List<SortOption>
        {
            new ("AmountAsc", "DebtView.SortOption.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOption.AmountDesc"),
            new ("DateAsc", "DebtView.SortOption.DateAsc"),
            new ("DateDesc", "DebtView.SortOption.DateDesc")
        };

        SelectedSortOrder = SortOptions[0];
    }

    public void ApplySorting()
    {
        if (Debts == null || !Debts.Any()) return;

        string selectedKey = SelectedSortOrder?.Key ?? "DateAsc";

        // 1. Provedeme seřazení do dočasného listu (v paměti)
        List<DebtItemViewModel> sortedList = selectedKey switch
        {
            "AmountAsc" => [.. Debts.OrderBy(d => d.Amount)],
            "AmountDesc" => [.. Debts.OrderByDescending(d => d.Amount)],
            "DateAsc" => [.. Debts.OrderBy(d => d.Timestamp)],
            "DateDesc" => [.. Debts.OrderByDescending(d => d.Timestamp)],
            _ => [.. Debts.OrderBy(d => d.Timestamp)]
        };

        // 2. Aktualizace kolekce
        // Tip: Pokud používáte DynamicData, použijte .Edit() pro hromadnou změnu
        Debts.Clear();
        foreach (var item in sortedList)
        {
            Debts.Add(item);
        }
    }

    partial void OnSelectedSortOrderChanged(SortOption value)
    {
        ApplySorting();
    }

    public void GetDebtsFromRepo()
    {
        List<TransactionDTO> transactions = ToDoRepo.GetTransactions("debt");

        Debts.Clear();

        foreach (var transaction in transactions)
        {
            Debts.Add(new DebtItemViewModel(transaction));
        }
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