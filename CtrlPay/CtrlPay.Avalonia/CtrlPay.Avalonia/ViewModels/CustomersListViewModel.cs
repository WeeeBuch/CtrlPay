using CommunityToolkit.Mvvm.ComponentModel;
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

        foreach (var dto in data)
        {
            var existingVm = Customers.FirstOrDefault(vm => vm.Model.Id == dto.Id);
            if (existingVm != null)
            {
                if (!existingVm.Editing)
                {
                    existingVm.Model = dto;
                }
                resultList.Add(existingVm);
            }
            else
            {
                resultList.Add(new CustomerPieceViewModel(dto));
            }
        }

        // Odstranění přebytečných
        var toRemove = Customers.Where(c => !resultList.Contains(c)).ToList();
        foreach (var item in toRemove) Customers.Remove(item);

        // Uspořádání podle výsledků filtru
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