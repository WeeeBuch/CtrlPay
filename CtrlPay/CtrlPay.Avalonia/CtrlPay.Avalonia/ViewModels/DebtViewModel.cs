using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CtrlPay.Avalonia.ViewModels;

public partial class DebtItemViewModel : ObservableObject
{
    [ObservableProperty] private string _description;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _currency = "XMR";

    public DebtItemViewModel(string desc, decimal amount)
    {
        _description = desc;
        _amount = amount;
    }

    [RelayCommand]
    private void PayFromCredit() { /* Implementace platby */ }

    [RelayCommand]
    private void GenerateAddress() { /* Implementace generování adresy */ }
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
        Debts.Add(new DebtItemViewModel("Popisek ale ne moc dlouhej", 50.000000000000m));
        Debts.Add(new DebtItemViewModel("Server hosting - únor", 0.152500000000m));

        SortOptions = new List<SortOption>
        {
            new ("AmountAsc", "DebtView.SortOptions.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOptions.AmountDesc"),
            new ("DateAsc", "DebtView.SortOptions.DateAsc"),
            new ("DateDesc", "DebtView.SortOptions.DateDesc")
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