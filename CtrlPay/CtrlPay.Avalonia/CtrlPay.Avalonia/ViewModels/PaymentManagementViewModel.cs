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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace CtrlPay.Avalonia.ViewModels;

public partial class PaymentManagementViewModel : ViewModelBase
{
    public class StatusFilterItem
    {
        public string Name { get; set; } = string.Empty;
        public StatusEnum? Value { get; set; }
        public override string ToString() => Name;
    }

    [ObservableProperty] private SortOption selectedSortOrder;
    [ObservableProperty] private List<SortOption> sortOptions;
    [ObservableProperty] private string searchTerm;
    [ObservableProperty] private bool _isEditing;

    [ObservableProperty] private RangeObservableCollection<FrontendPaymentDTO> payments = [];
    [ObservableProperty] private FrontendPaymentDTO? selectedPayment;
    [ObservableProperty] private bool canBeDeleted = false;

    [ObservableProperty] private ObservableCollection<FrontendCustomerDTO> customers = [];

    [ObservableProperty] private StatusFilterItem? selectedStatusItem;
    public ObservableCollection<StatusFilterItem> Statuses { get; } = new();

    public PaymentManagementViewModel() 
    {
        InitializeStatuses();

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

        TranslationManager.LanguageChanged.Add(RefreshTranslations);
    }

    private void RefreshTranslations()
    {
        var currentStatusValue = SelectedStatusItem?.Value;
        InitializeStatuses();
        SelectedStatusItem = Statuses.FirstOrDefault(s => s.Value == currentStatusValue) ?? Statuses[0];
        ApplySorting(SelectedSortOrder?.Key);
    }

    private void InitializeStatuses()
    {
        Statuses.Clear();
        Statuses.Add(new StatusFilterItem 
        { 
            Name = TranslationManager.GetString("Accountant.Transactions.Filter.AllStatuses"), 
            Value = null 
        });

        foreach (StatusEnum status in Enum.GetValues(typeof(StatusEnum)))
        {
            string translationKey = $"Transaction.Status.{status}";
            string translatedName = TranslationManager.GetString(translationKey);
            
            if (string.IsNullOrEmpty(translatedName) || translatedName == translationKey)
            {
                translatedName = status.ToString();
            }

            Statuses.Add(new StatusFilterItem { Name = translatedName, Value = status });
        }

        SelectedStatusItem = Statuses[0];
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
            await AccountantRepo.UpdatePayment(SelectedPayment);

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
        await AccountantRepo.DeletePayment(SelectedPayment);
    }

    public string GetCustomerName(int? id) => Customers.FirstOrDefault(c => c.Id == id)?.FullName ?? "Neznámý";

    public void ApplySorting(string? sortingMethod)
    {
        AppLogger.Info($"Sorting Payments by: {sortingMethod}");

        var newData = AccountantRepo.GetSortedPayments(sortingMethod);

        // Filtrování podle stavu
        if (SelectedStatusItem?.Value != null)
        {
            newData = AccountantRepo.GetPaymentsByStatus(SelectedStatusItem.Value);
        }

        // Filtrování podle vyhledávání
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            newData = newData.Where(vm => vm.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

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

    partial void OnSearchTermChanged(string value) => ApplySorting(SelectedSortOrder?.Key);
    partial void OnSelectedStatusItemChanged(StatusFilterItem? value) => ApplySorting(SelectedSortOrder?.Key);

    [RelayCommand]
    public async Task ConvertOverpaymentToCredit()
    {
        if (SelectedPayment == null) return;
        if (SelectedPayment.Status != StatusEnum.Overpaid) return;

        bool success = await AccountantRepo.ConvertOverpaymentToCredit(SelectedPayment);
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

    [RelayCommand]
    public async Task SendReminder()
    {
        if (SelectedPayment == null) return;
        bool success = await PaymentRepo.SendReminder(SelectedPayment);
        if (success)
        {
            AppLogger.Info($"Upomínka pro platbu {SelectedPayment.Id} úspěšně odeslána.");
        }
        else
        {
            AppLogger.Warning($"Odeslání upomínky selhalo.");
        }
    }
}
