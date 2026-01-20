using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    }

    [RelayCommand]
    private void PayFromCredit() => ToDoRepo.PayFromCredit(TransactionDTOBase);

    [RelayCommand]
    private void GenerateAddress() 
    { 
        /* Implementace generování adresy */ 
        string addr = ToDoRepo.GetOneTimeAddress(TransactionDTOBase);

        QrCodeViewModel vm = new(addr);
        QrCodeView view = new() { DataContext = vm };

        var window = new Window
        {
            Content = view,
            Title = "QR Kód",
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        window.Show();
        /*
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            window.ShowDialog(desktop.MainWindow!);
        }*/
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
    public RangeObservableCollection<DebtItemViewModel> Debts { get; } = new();

    // Tohle se pak přesune do repa
    public List<FrontendTransactionDTO> LoadedTransactions { get; set; } = new();

    [ObservableProperty]
    private bool payableChecked;

    [ObservableProperty]
    private SortOption selectedSortOrder;

    [ObservableProperty]
    private List<SortOption> sortOptions;

    public DebtViewModel()
    {
        _ = GetDebtsFromRepo();

        SortOptions = new List<SortOption>
        {
            new ("AmountAsc", "DebtView.SortOption.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOption.AmountDesc"),
            new ("DateAsc", "DebtView.SortOption.DateAsc"),
            new ("DateDesc", "DebtView.SortOption.DateDesc")
        };

        SelectedSortOrder = SortOptions[0];

        ApplySorting(SelectedSortOrder.Key);

        UpdateHandler.CreditAvailableUpdateActions.Add(OnCreditChanged);
        UpdateHandler.NewTransactionAddedActions.Add(TransactionsUpdated);
    }

    public void ApplySorting(string? sortingMethod)
    {
        // 1. Získáme aktuální sumu kreditů pro porovnání
        decimal creditAmount = ToDoRepo.GetTransactionSums("credits");
        string selectedKey = sortingMethod ?? SelectedSortOrder.Key;

        // 2. VŽDY filtrujeme z LoadedTransactions (tam jsou všechny dluhy z repa)
        List<FrontendTransactionDTO> filteredData = [.. 
            LoadedTransactions.Where(t => !PayableChecked || t.Amount <= creditAmount)];

        List<FrontendTransactionDTO> sortedDTOs;
        
        sortedDTOs = selectedKey switch
        {
            "AmountAsc" => [.. filteredData.OrderBy(d => d.Amount)],
            "AmountDesc" => [.. filteredData.OrderByDescending(d => d.Amount)],
            "DateAsc" => [.. filteredData.OrderBy(d => d.Timestamp)],
            "DateDesc" => [.. filteredData.OrderByDescending(d => d.Timestamp)],
            _ => [.. filteredData.OrderBy(d => d.Timestamp)]
        };
        

        // 4. Synchronizace s UI - Reuse existujících ViewModelů
        // Tady je ten trik: Hledáme v Debts, jestli už tam VM je, 
        // pokud ne, vytvoříme nový.
        var resultList = new List<DebtItemViewModel>();

        foreach (var dto in sortedDTOs)
        {
            var existingVm = Debts.FirstOrDefault(vm =>
                vm.TransactionDTOBase == dto);

            if (existingVm != null)
            {
                existingVm.UpdateCreditAmount(creditAmount);
                resultList.Add(existingVm);
            }
            else
            {
                var newVm = new DebtItemViewModel(dto);
                newVm.UpdateCreditAmount(creditAmount);
                resultList.Add(newVm);
            }
        }
                
        Debts.ReplaceAll(resultList);
        
    }

    public void OnCreditChanged(decimal amount)
    {
        ApplySorting(null);
    }

    partial void OnPayableCheckedChanged(bool value)
    {
        ApplySorting(null);
    }

    public void TransactionsUpdated()
    {
        _ = GetDebtsFromRepo();
        ApplySorting(null);
    }

    partial void OnSelectedSortOrderChanged(SortOption value)
    {
        ApplySorting(value.Key);
    }

    private void UpdateDebts(List<FrontendTransactionDTO> ltDTO)
    {
        LoadedTransactions = ltDTO;
        Debts.ReplaceAll([.. ltDTO.Select(p => new DebtItemViewModel(p))]);
    }

    public async Task GetDebtsFromRepo()
    {
        var loadedPayments = await PaymentRepo.GetPayments(default);

        // Vynucení aktualizace na UI vlákně
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            UpdateDebts(loadedPayments);
        });
    }
}

public partial class SortOption : ObservableObject
{
    public string Key { get; set; }

    [ObservableProperty]
    private string? displayName;

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

public class RangeObservableCollection<T> : ObservableCollection<T>
{
    public void ReplaceAll(IEnumerable<T> items)
    {
        Items.Clear(); // Interní seznam vymažeme bez notifikace
        foreach (var item in items) Items.Add(item);

        // Vyvoláme notifikaci pro UI jen JEDNOU (Reset)
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}