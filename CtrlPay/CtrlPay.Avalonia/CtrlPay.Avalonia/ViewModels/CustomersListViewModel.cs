using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomersListViewModel : ViewModelBase
{
    [ObservableProperty] private string? _searchTextInput;
    public ObservableCollection<CustomerPieceViewModel> Customers { get; } = new();

    public CustomersListViewModel()
    {
        LoadCustomers();
        UpdateHandler.UpdatedCustomers.Add(() =>
        {
            LoadCustomers();
        });
    }

    [RelayCommand]
    public void AddCustomer()
    {
        // Pokud už nahoře jeden rozdělaný (nový) je, nepouštěj další
        if (Customers.Any(c => c.Editing && c.Model.Id == 0))
            return;

        var newDto = new FrontendCustomerDTO { Physical = true, FirstName = "", LastName = "", Company = "" };
        var newVm = new CustomerPieceViewModel(newDto) { Editing = true, IsExpanded = true };
        newVm.Model.BeginEdit();

        Customers.Insert(0, newVm);
    }

    partial void OnSearchTextInputChanged(string? value)
    {
        LoadCustomers();
    }

    private void LoadCustomers()
    {
        // Pokud je vyhledávání prázdné, vezmeme všechny, jinak filtrujeme
        List<FrontendCustomerDTO> data;

        if (string.IsNullOrWhiteSpace(SearchTextInput))
        {
            data = CustomerRepo.GetCustomers();
        }
        else
        {
            data = CustomerRepo.Filter(SearchTextInput);
        }

        SyncCollections(data);
    }

    private void SyncCollections(List<FrontendCustomerDTO> data)
    {
        var resultList = new List<CustomerPieceViewModel>();

        // 1. Nejdřív si do výsledků přidáme všechny, co právě editujeme 
        // (včetně těch nových s ID 0)
        var editingNow = Customers.Where(c => c.Editing).ToList();
        resultList.AddRange(editingNow);

        // 2. Projdeme data z databáze
        foreach (var dto in data)
        {
            // Pokud už v seznamu je (a není to ten, co právě editujeme - ten už tam je z bodu 1)
            var existingVm = Customers.FirstOrDefault(vm => vm.Model.Id == dto.Id);

            if (existingVm != null)
            {
                if (!existingVm.Editing)
                {
                    existingVm.Model = dto;
                }

                // Přidáme ho do resultListu, pokud tam ještě není (nebyl v editingNow)
                if (!resultList.Contains(existingVm))
                {
                    resultList.Add(existingVm);
                }
            }
            else
            {
                // Je to nový zákazník z DB, kterého ještě v UI nemáme
                resultList.Add(new CustomerPieceViewModel(dto));
            }
        }

        // 3. Smazání těch, co nejsou v resultListu 
        // (Teď už to nesmaže ty s Editing = true, protože jsou v resultListu)
        var toRemove = Customers.Where(c => !resultList.Contains(c)).ToList();
        foreach (var item in toRemove) Customers.Remove(item);

        // 4. Seřazení (ponecháme tvou logiku Move/Insert)
        for (int i = 0; i < resultList.Count; i++)
        {
            var targetVm = resultList[i];
            if (i >= Customers.Count)
            {
                Customers.Add(targetVm);
            }
            else if (Customers[i] != targetVm)
            {
                int oldIndex = Customers.IndexOf(targetVm);
                if (oldIndex != -1) Customers.Move(oldIndex, i);
                else Customers.Insert(i, targetVm);
            }
        }
    }
}
