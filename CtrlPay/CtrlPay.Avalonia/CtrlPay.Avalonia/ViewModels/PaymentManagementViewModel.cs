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
    }

    [RelayCommand]
    public void CancelEdit(FrontendPaymentDTO payment)
    {
        payment.CancelEdit();
        IsEditing = false;
    }

    public string GetCustomerName(int? id) => Customers.FirstOrDefault(c => c.Id == id)?.FullName ?? "Neznámý";

    public void ApplySorting(string? sortingMethod)
    {
        AppLogger.Info($"Sorting Payments by: {sortingMethod}");

        // 1. Získáme nová data z repozitáře
        var newData = AccountantPaymentRepo.GetSortedPayments(sortingMethod);

        if (ShowOnlyOverpaid)
            newData = newData.Where(p => p.Status == StatusEnum.Overpaid).ToList();

        // 2. Synchronizace kolekcí (podobně jako u Customers)
        // Uložíme si ID aktuálně vybrané platby
        var selectedId = SelectedPayment?.Id;

        // Odstraníme ty, co už v nových datech nejsou
        var toRemove = Payments.Where(p => !newData.Any(n => n.Id == p.Id)).ToList();
        foreach (var item in toRemove) Payments.Remove(item);

        // Aktualizujeme stávající a přidáme nové
        for (int i = 0; i < newData.Count; i++)
        {
            var dto = newData[i];
            var existing = Payments.FirstOrDefault(p => p.Id == dto.Id);

            if (existing != null)
            {
                // Aktualizujeme data v existujícím objektu bez ztráty reference
                // POZOR: Pokud FrontendPaymentDTO neimplementuje INotifyPropertyChanged, 
                // změny se v UI neprojeví hned. Ale reference zůstane.
                existing.CustomerId = dto.CustomerId;
                existing.ExpectedAmountXMR = dto.ExpectedAmountXMR;
                existing.PaidAmountXMR = dto.PaidAmountXMR;
                existing.Status = dto.Status;
                existing.Title = dto.Title;
                existing.DueDate = dto.DueDate;

                // Posuneme na správnou pozici podle řazení
                int oldIndex = Payments.IndexOf(existing);
                if (oldIndex != i) Payments.Move(oldIndex, i);
            }
            else
            {
                // Nová platba
                Payments.Insert(i, dto);
            }
        }

        // 3. Obnova výběru
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
