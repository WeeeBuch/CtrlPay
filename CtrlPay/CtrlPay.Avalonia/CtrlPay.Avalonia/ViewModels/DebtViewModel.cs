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
public partial class DebtViewModel : ViewModelBase
{
    public RangeObservableCollection<TransactionItem> Debts { get; } = [];

    [ObservableProperty] private bool payableChecked;
    [ObservableProperty] private SortOption selectedSortOrder;
    [ObservableProperty] private List<SortOption> sortOptions;
    [ObservableProperty] private string searchTerm;

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
        var resultList = new List<TransactionItem>();

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

    partial void OnSearchTermChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ApplySorting(null);
        }
        else
        {
            ApplySorting(null);
            var resultList = Debts.Where(vm => vm.Description.ToLower().Contains(value.ToLower())).ToList();
            Debts.ReplaceAll(resultList);
        }
    }
}



