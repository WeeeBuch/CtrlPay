using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CtrlPay.Avalonia.ViewModels;

public partial class AccountantTransactionsViewModel : ViewModelBase
{
    [ObservableProperty] private bool isFlowView;
    [ObservableProperty] private string searchTerm = string.Empty;
    [ObservableProperty] private string? selectedCustomer;

    public ObservableCollection<AccountantTransactionDTO> AllTransactions { get; } = new();
    public RangeObservableCollection<AccountantTransactionDTO> FilteredTransactions { get; } = new();
    
    // Pro Flow View (Incoming vlevo, Outgoing vpravo)
    public RangeObservableCollection<AccountantTransactionDTO> IncomingTransactions { get; } = new();
    public RangeObservableCollection<AccountantTransactionDTO> OutgoingTransactions { get; } = new();

    public ObservableCollection<string> Customers { get; } = new();

    public AccountantTransactionsViewModel()
    {
        LoadData();

        // Registrace pro automatické aktualizace
        UpdateHandler.NewPaymentsAddedActions.Add(LoadData);
        UpdateHandler.NewDebtsAddedActions.Add(LoadData);
        UpdateHandler.UpdatedCustomers.Add(LoadData);
    }

    private void LoadData()
    {
        AppLogger.Info("AccountantTransactionsViewModel: Loading data...");
        var data = ToDoRepo.GetMockAccountantTransactions();
        
        AllTransactions.Clear();
        foreach (var t in data) AllTransactions.Add(t);
        
        // Aktualizace seznamu zákazníků (zachováme výběr, pokud je to možné)
        var currentSelection = SelectedCustomer;
        var newCustomerList = AllTransactions.Select(t => t.CustomerName)
                                            .Distinct()
                                            .OrderBy(c => c)
                                            .ToList();
        newCustomerList.Insert(0, "All Customers");

        if (!Customers.SequenceEqual(newCustomerList))
        {
            Customers.Clear();
            foreach (var c in newCustomerList) Customers.Add(c);
            
            if (currentSelection != null && Customers.Contains(currentSelection))
                SelectedCustomer = currentSelection;
            else
                SelectedCustomer = Customers[0];
        }

        ApplyFilters();
        AppLogger.Info($"AccountantTransactionsViewModel: Loaded {AllTransactions.Count} transactions.");
    }

    [RelayCommand]
    private void ToggleView()
    {
        IsFlowView = !IsFlowView;
    }

    partial void OnSearchTermChanged(string value) => ApplyFilters();
    partial void OnSelectedCustomerChanged(string? value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = AllTransactions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filtered = filtered.Where(t => 
                t.CustomerName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedCustomer != "All Customers" && !string.IsNullOrEmpty(SelectedCustomer))
        {
            filtered = filtered.Where(t => t.CustomerName == SelectedCustomer);
        }

        var list = filtered.ToList();
        
        // Důležité: ReplaceAll zajistí, že se UI dozví o změně celé kolekce naráz
        FilteredTransactions.ReplaceAll(list);

        // Rozdělení pro Flow View
        IncomingTransactions.ReplaceAll([.. list.Where(t => t.Type == Entities.TransactionTypeEnum.Incoming)]);
        OutgoingTransactions.ReplaceAll([.. list.Where(t => t.Type == Entities.TransactionTypeEnum.Outgoing)]);
    }
}
