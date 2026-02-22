using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    [ObservableProperty] private bool _isEditing;

    [ObservableProperty] private RangeObservableCollection<FrontendPaymentDTO> payments = [];
    [ObservableProperty] private FrontendPaymentDTO? selectedPayment;
    [ObservableProperty] private bool canBeDeleted = false;

    [ObservableProperty] private ObservableCollection<FrontendCustomerDTO> customers = [];

    [ObservableProperty] private bool showOnlyOverpaid;

    partial void OnShowOnlyOverpaidChanged(bool value) => ApplySorting(SelectedSortOrder?.Key);

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

        RefreshAllData();

        UpdateHandler.UpdatedCustomers.Add(() => SyncCustomers(CustomerRepo.GetCustomers()));
        UpdateHandler.NewPaymentsAddedActions.Add(() => ApplySorting(null));
    }

    [RelayCommand]
    public void AddNewPayment()
    {
        FrontendPaymentDTO paymentDTO = new() 
        {  
            CreatedAt = DateTime.Now,
            Id = 0,
            Status = StatusEnum.Created,
            Title = string.Empty
        };

        paymentDTO.BeginEdit();
        IsEditing = true;

        Payments.Insert(0, paymentDTO);

        SelectedPayment = paymentDTO;
    }

    private void RefreshAllData()
    {
        SyncCustomers(CustomerRepo.GetCustomers());
        ApplySorting(SelectedSortOrder?.Key);
    }

    private void SyncCustomers(List<FrontendCustomerDTO> freshData)
    {
        // Odstraníme ty, co už neexistují
        var toRemove = Customers.Where(c => !freshData.Any(n => n.Id == c.Id)).ToList();
        foreach (var item in toRemove) Customers.Remove(item);

        foreach (var dto in freshData)
        {
            var existing = Customers.FirstOrDefault(c => c.Id == dto.Id);
            if (existing != null)
            {
                // Aktualizujeme data v existujícím objektu (pokud jsi i u Customera implementoval ObservableObject)
                existing.FirstName = dto.FirstName;
                existing.LastName = dto.LastName;
                existing.Company = dto.Company;
                existing.Physical = dto.Physical;
            }
            else
            {
                Customers.Add(dto);
            }
        }
    }

    public async Task SavePaymentAsync()
    {
        IsEditing = false;
        if (SelectedPayment == null) return;

        try
        {
            await AccountantPaymentRepo.UpdatePayment(SelectedPayment);

            RefreshAllData();
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Chyba při ukládání platby: {ex.Message}");
        }
    }

    [RelayCommand]
    public void StartEdit(FrontendPaymentDTO payment)
    {
        payment.BeginEdit();
        IsEditing = true;
        CanBeDeleted = payment.Status == StatusEnum.WaitingForPayment || payment.Status == StatusEnum.Pending;
    }

    [RelayCommand]
    public void CancelEdit(FrontendPaymentDTO payment)
    {
        payment.CancelEdit();
        IsEditing = false;
        CanBeDeleted = false;
    }

    [RelayCommand]
    public async Task DeletePayment()
    {
        await AccountantPaymentRepo.DeletePayment(SelectedPayment);
    }

    public string GetCustomerName(int? id) => Customers.FirstOrDefault(c => c.Id == id)?.FullName ?? "Neznámý";

    public void ApplySorting(string? sortingMethod)
    {
        AppLogger.Info($"Sorting Payments by: {sortingMethod}");

        var newData = AccountantPaymentRepo.GetSortedPayments(sortingMethod);

        if (ShowOnlyOverpaid)
            newData = newData.Where(p => p.Status == StatusEnum.Overpaid).ToList();

        var selectedId = SelectedPayment?.Id;

        var toRemove = Payments.Where(p => !newData.Any(n => n.Id == p.Id))
                               .Where(p => !(IsEditing && p.Id == 0)).ToList();
        foreach (var item in toRemove) Payments.Remove(item);

        for (int i = 0; i < newData.Count; i++)
        {
            var dto = newData[i];
            var existing = Payments.FirstOrDefault(p => p.Id == dto.Id);

            if (existing != null)
            {
                existing.CustomerId = dto.CustomerId;
                existing.ExpectedAmountXMR = dto.ExpectedAmountXMR;
                existing.PaidAmountXMR = dto.PaidAmountXMR;
                existing.Status = dto.Status;
                existing.Title = dto.Title;
                existing.DueDate = dto.DueDate;

                int oldIndex = Payments.IndexOf(existing);
                if (oldIndex != i) Payments.Move(oldIndex, i);
            }
            else
            {
                Payments.Insert(i, dto);
            }
        }

        if (selectedId.HasValue)
        {
            SelectedPayment = Payments.FirstOrDefault(p => p.Id == selectedId.Value);
        }

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

        if (ShowOnlyOverpaid)
            resultList = resultList.Where(p => p.Status == StatusEnum.Overpaid).ToList();

        Payments.ReplaceAll(resultList);
    }

    [RelayCommand]
    public async Task ConvertOverpaymentToCredit()
    {
        if (SelectedPayment == null) return;
        if (SelectedPayment.Status != StatusEnum.Overpaid) return;

        bool success = await AccountantPaymentRepo.ConvertOverpaymentToCredit(SelectedPayment);
        if (success)
        {
            AppLogger.Info($"Přebytek platby {SelectedPayment.Id} úspěšně převeden do kreditů.");
            RefreshAllData();
        }
        else
        {
            AppLogger.Warning($"Převod přebytku selhal.");
        }
    }
}
