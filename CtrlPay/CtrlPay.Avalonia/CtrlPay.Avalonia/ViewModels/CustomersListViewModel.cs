using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomersListViewModel : ViewModelBase
{
    public ObservableCollection<CustomerPieceViewModel> Customers { get; } = new();

    public CustomersListViewModel()
    {
        LoadCustomers();
    }

    private void LoadCustomers()
    {
        List<FrontendCustomerDTO> data = Repos.CustomerRepo.GetCustomers();

        Customers.Clear();

        foreach (var customer in data)
        {
            Customers.Add(new(customer));
        }
    }
}