using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class TransactionViewModel : ViewModelBase
{
    public RangeObservableCollection<TransactionItem> Debts { get; } = [];

    [ObservableProperty] private SortOption selectedSortOrder;
    [ObservableProperty] private List<SortOption> sortOptions;
    [ObservableProperty] private string searchTerm;

    public TransactionViewModel()
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
        AppLogger.Info($"Sorting transactions by: {sortingMethod}");
        var resultList = new List<TransactionItem>();

        foreach (var dto in TransactionRepo.GetSortedTransactions(sortingMethod))
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
        AppLogger.Info($"Sorting finished.");
    }

    public void OnCreditChanged(decimal amount) => ApplySorting(null);
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
