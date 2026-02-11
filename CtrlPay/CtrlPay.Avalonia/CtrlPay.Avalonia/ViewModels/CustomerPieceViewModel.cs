using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Entities;
using System.Reflection;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomerPieceViewModel : ViewModelBase
{
    [ObservableProperty] private Customer _model;

    public CustomerPieceViewModel(Customer customer)
    {
        _model = customer;
    }

    public string FullName => $"{Model.FirstName} {Model.LastName}";
}