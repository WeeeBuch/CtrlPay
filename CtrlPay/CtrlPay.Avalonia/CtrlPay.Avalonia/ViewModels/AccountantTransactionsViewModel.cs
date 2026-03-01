using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CtrlPay.Avalonia.ViewModels;

public partial class AccountantTransactionsViewModel : ViewModelBase
{
    public class StatusFilterItem
    {
        public string Name { get; set; } = string.Empty;
        public StatusEnum? Value { get; set; }
        public override string ToString() => Name;
    }

    [ObservableProperty] private bool isFlowView;
    [ObservableProperty] private string toggleViewText = string.Empty;
    [ObservableProperty] private string searchTerm = string.Empty;
    [ObservableProperty] private string? selectedCustomer;
    [ObservableProperty] private StatusFilterItem? selectedStatusItem;
    
    // Řazení
    [ObservableProperty] private string sortColumn = "Date";
    [ObservableProperty] private bool isSortAscending = false;

    public ObservableCollection<AccountantTransactionDTO> AllTransactions { get; } = new();
    public RangeObservableCollection<AccountantTransactionDTO> FilteredTransactions { get; } = new();
    
    public RangeObservableCollection<AccountantTransactionDTO> IncomingTransactions { get; } = new();
    public RangeObservableCollection<AccountantTransactionDTO> OutgoingTransactions { get; } = new();

    public ObservableCollection<string> Customers { get; } = new();
    public ObservableCollection<StatusFilterItem> Statuses { get; } = new();

    public AccountantTransactionsViewModel()
    {
        InitializeStatuses();
        UpdateToggleViewText();
        LoadData();
        
        TranslationManager.LanguageChanged.Add(() => 
        {
            RefreshTranslations();
        });

        UpdateHandler.NewPaymentsAddedActions.Add(LoadData);
        UpdateHandler.NewDebtsAddedActions.Add(LoadData);
        UpdateHandler.UpdatedCustomers.Add(LoadData);
    }

    private void RefreshTranslations()
    {
        // Uložíme si aktuálně vybraný stav
        var currentStatusValue = SelectedStatusItem?.Value;
        
        InitializeStatuses();
        UpdateToggleViewText();
        
        // Zkusíme vybrat ten samý stav v novém jazyce
        SelectedStatusItem = Statuses.FirstOrDefault(s => s.Value == currentStatusValue) ?? Statuses[0];

        // Znovu načteme data pro aktualizaci textů (např. "Všichni zákazníci")
        LoadData();
    }

    private void UpdateToggleViewText()
    {
        ToggleViewText = IsFlowView 
            ? TranslationManager.GetString("Accountant.Transactions.View.Table") 
            : TranslationManager.GetString("Accountant.Transactions.View.Flow");
    }

    partial void OnIsFlowViewChanged(bool value) => UpdateToggleViewText();

    private void InitializeStatuses()
    {
        Statuses.Clear();
        
        // Položka pro "Všechny stavy"
        Statuses.Add(new StatusFilterItem 
        { 
            Name = TranslationManager.GetString("Accountant.Transactions.Filter.AllStatuses"), 
            Value = null 
        });

        // Dynamické načtení všech stavů z Enumu
        foreach (StatusEnum status in Enum.GetValues(typeof(StatusEnum)))
        {
            // Zkusíme najít překlad pod klíčem "Transaction.Status.[NázevStavu]"
            string translationKey = $"Transaction.Status.{status}";
            string translatedName = TranslationManager.GetString(translationKey);
            
            // Pokud překlad neexistuje, TranslationManager vrací klíč nebo null (podle implementace), 
            // tak jako fallback použijeme název enumu
            if (string.IsNullOrEmpty(translatedName) || translatedName == translationKey)
            {
                translatedName = status.ToString();
            }

            Statuses.Add(new StatusFilterItem { Name = translatedName, Value = status });
        }

        SelectedStatusItem = Statuses[0];
    }

    [RelayCommand]
    private void SortBy(string column)
    {
        if (SortColumn == column)
        {
            IsSortAscending = !IsSortAscending;
        }
        else
        {
            SortColumn = column;
            IsSortAscending = true;
        }
        ApplyFilters();
    }

    private void LoadData()
    {
        var data = AccountantRepo.GetAccountantTransactions();

        AllTransactions.Clear();
        foreach (var t in data) AllTransactions.Add(t);
        
        var currentSelection = SelectedCustomer;
        var newCustomerList = AllTransactions.Select(t => t.CustomerName).Distinct().OrderBy(c => c).ToList();
        newCustomerList.Insert(0, TranslationManager.GetString("Accountant.Transactions.AllCustomers"));

        if (!Customers.SequenceEqual(newCustomerList))
        {
            Customers.Clear();
            foreach (var c in newCustomerList) Customers.Add(c);
            SelectedCustomer = Customers.Contains(currentSelection ?? "") ? currentSelection : Customers[0];
        }

        ApplyFilters();
    }

    [RelayCommand]
    private void ToggleView() => IsFlowView = !IsFlowView;

    partial void OnSearchTermChanged(string value) => ApplyFilters();
    partial void OnSelectedCustomerChanged(string? value) => ApplyFilters();
    partial void OnSelectedStatusItemChanged(StatusFilterItem? value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = AllTransactions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filtered = filtered.Where(t => 
                t.CustomerName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedCustomer != TranslationManager.GetString("Accountant.Transactions.AllCustomers") && !string.IsNullOrEmpty(SelectedCustomer))
        {
            filtered = filtered.Where(t => t.CustomerName == SelectedCustomer);
        }

        if (SelectedStatusItem?.Value != null)
        {
            filtered = filtered.Where(t => t.State == SelectedStatusItem.Value);
        }

        // Aplikace řazení
        filtered = SortColumn switch
        {
            "Id" => IsSortAscending ? filtered.OrderBy(t => t.Id) : filtered.OrderByDescending(t => t.Id),
            "Date" => IsSortAscending ? filtered.OrderBy(t => t.Timestamp) : filtered.OrderByDescending(t => t.Timestamp),
            "Customer" => IsSortAscending ? filtered.OrderBy(t => t.CustomerName) : filtered.OrderByDescending(t => t.CustomerName),
            "Amount" => IsSortAscending ? filtered.OrderBy(t => t.Amount) : filtered.OrderByDescending(t => t.Amount),
            "Status" => IsSortAscending ? filtered.OrderBy(t => t.State) : filtered.OrderByDescending(t => t.State),
            _ => filtered.OrderByDescending(t => t.Timestamp)
        };

        var list = filtered.ToList();
        FilteredTransactions.ReplaceAll(list);

        IncomingTransactions.ReplaceAll(list.Where(t => t.Type == Entities.TransactionTypeEnum.Incoming).ToList());
        OutgoingTransactions.ReplaceAll(list.Where(t => t.Type == Entities.TransactionTypeEnum.Outgoing).ToList());
    }
}
