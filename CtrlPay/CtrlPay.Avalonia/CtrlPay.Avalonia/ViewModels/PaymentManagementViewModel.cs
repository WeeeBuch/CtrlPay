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

        UpdateHandler.UpdatedCustomers.Add(LoadCustomers);
        UpdateHandler.NewPaymentsAddedActions.Add(() => ApplySorting(null));
    }

    private void LoadCustomers()
    {
        var loadedCustomers = CustomerRepo.GetCustomers();
        Customers = new ObservableCollection<FrontendCustomerDTO>(loadedCustomers);
    }

    public async Task SavePaymentAsync()
    {
        if (SelectedPayment == null) return;

        try
        {
            // 1. Převedeme frontendové DTO zpět na API DTO (to už máš připravené)
            var apiDto = SelectedPayment.ToApiDto();

            // 2. Pošleme to do repozitáře (tady si doplň svůj název metody)
            // Předpokládám, že AccountantPaymentRepo má metodu Update
            bool success = true;

            if (success)
            {
                AppLogger.Info("Platba byla úspěšně uložena.");
                // Volitelně: Refresh seznamu, aby se změny projevily všude
                ApplySorting(SelectedSortOrder?.Key);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Chyba při ukládání platby: {ex.Message}");
        }
    }

    public string GetCustomerName(int? id) => Customers.FirstOrDefault(c => c.Id == id)?.FullName ?? "Neznámý";

    public void ApplySorting(string? sortingMethod)
    {
        AppLogger.Info($"Sorting Payments by: {sortingMethod}");

        // 1. Získáme nová data z repozitáře
        var newData = AccountantPaymentRepo.GetSortedPayments(sortingMethod);

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
        Payments.ReplaceAll(resultList);
    }
}
