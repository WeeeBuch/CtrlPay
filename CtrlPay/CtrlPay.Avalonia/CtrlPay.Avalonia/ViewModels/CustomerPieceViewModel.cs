using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Reflection;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomerPieceViewModel : ViewModelBase
{
    [ObservableProperty] private FrontendCustomerDTO _model;
    [ObservableProperty] private bool _editing = false;

    public CustomerPieceViewModel(FrontendCustomerDTO customer)
    {
        _model = customer;
    }

    public string FullName => $"{Model.FirstName} {Model.LastName}";

    [RelayCommand]
    public void StartEdit()
    {
        Editing = true;
    }
    [RelayCommand]
    public void DeleteEditCommand()
    {
        Editing = false;
        CustomerRepo.DeleteCustomer(Model);
    }

    [RelayCommand]
    public void EndEdit()
    {
        Editing = false;
        var temp = Model;
        Model = null!;
        Model = temp;

        OnPropertyChanged(nameof(FullName));
    }

    [RelayCommand]
    public void CancelEdit()
    {
        Editing = false;
        OnPropertyChanged(nameof(FullName));
    }
}