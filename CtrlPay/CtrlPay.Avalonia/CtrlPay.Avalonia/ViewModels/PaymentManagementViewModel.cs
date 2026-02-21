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

    [ObservableProperty] private ObservableCollection<FrontendCustomerDTO> customers = [];

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
        LoadCustomers();
    }

    private void LoadCustomers()
    {
        var loadedCustomers = CustomerRepo.GetCustomers();
        Customers = new ObservableCollection<FrontendCustomerDTO>(loadedCustomers);
    }

    public async Task SaveCurrentPayment()
    {
        if (SelectedPayment != null)
        {
            // Volání tvého API/Repo pro update
            // await AccountantPaymentRepo.UpdatePayment(SelectedPayment.ToApiDto());
        }
    }

    public string GetCustomerName(int? id) => Customers.FirstOrDefault(c => c.Id == id)?.FullName ?? "Neznámý";

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
