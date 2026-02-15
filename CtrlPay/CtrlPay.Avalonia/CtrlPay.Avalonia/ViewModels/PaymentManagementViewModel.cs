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

    [ObservableProperty] private RangeObservableCollection<FrontendPaymentDTO> payments = [];
    [ObservableProperty] private FrontendPaymentDTO? selectedPayment;

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

        Payments.ReplaceAll(AccountantPaymentRepo.GetSortedPayments(sortingMethod));
        AppLogger.Info($"Sorting finished.");
    }

    public void TransactionsUpdated() => ApplySorting(null);
    partial void OnSelectedSortOrderChanged(SortOption value) => ApplySorting(value.Key);

    partial void OnSearchTermChanged(string value)
    {
        ApplySorting(null);

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var resultList = Payments.Where(vm => vm.Title.ToLower().Contains(value.ToLower())).ToList();
        Payments.ReplaceAll(resultList);
    }
}
