using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Repos.Frontend;
using System.Reflection;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomerPieceViewModel : ViewModelBase
{
    [ObservableProperty] private FrontendCustomerDTO _model;

    public CustomerPieceViewModel(FrontendCustomerDTO customer)
    {
        _model = customer;
    }

    public string FullName => $"{Model.FirstName} {Model.LastName}";
}