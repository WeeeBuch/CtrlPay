using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace CtrlPay.Avalonia.ViewModels;

public partial class PaymentManagementViewModel : ViewModelBase
{

    [ObservableProperty] private SortOption selectedSortOrder;
    [ObservableProperty] private List<SortOption> sortOptions;
    [ObservableProperty] private string searchTerm;

    [ObservableProperty] private RangeObservableCollection<TransactionItem> payments = [];
    [ObservableProperty] private TransactionItem? _selectedPayment;

    public PaymentManagementViewModel() 
    {
        SortOptions =
        [
            new ("AmountAsc", "DebtView.SortOption.AmountAsc"),
            new ("AmountDesc", "DebtView.SortOption.AmountDesc"),
            new ("DateAsc", "DebtView.SortOption.DateAsc"),
            new ("DateDesc", "DebtView.SortOption.DateDesc")
        ];

        SelectedSortOrder = SortOptions[0];

        ApplySorting(null);
    }

    public void ApplySorting(string? sortingMethod)
    {
        AppLogger.Info($"Sorting Payments by: {sortingMethod}");
        var resultList = new List<TransactionItem>();

        foreach (var dto in TransactionRepo.GetSortedTransactions(sortingMethod))
        {
            var existingVm = Payments.FirstOrDefault(vm =>
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

        Payments.ReplaceAll(resultList);
        AppLogger.Info($"Sorting finished.");
    }

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
            var resultList = Payments.Where(vm => vm.Description.ToLower().Contains(value.ToLower())).ToList();
            Payments.ReplaceAll(resultList);
        }
    }
}
