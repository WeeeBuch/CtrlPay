using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Reflection;
using System.Threading.Tasks;

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
        Model.BeginEdit();
    }
    [RelayCommand]
    public async Task DeleteEditCommand()
    {
        Editing = false;
        Model.EndEdit();
        await CustomerRepo.DeleteCustomer(Model);
        UpdateHandler.HandleUpdatedCustomers();
    }

    [RelayCommand]
    public async Task EndEditAsync()
    {
        Editing = false;
        Model.EndEdit();

        var temp = Model;
        Model = null!;
        Model = temp;

        await CustomerRepo.UpdateCustomer(Model);
        OnPropertyChanged(nameof(FullName));
    }

    [RelayCommand]
    public void CancelEdit()
    {
        Editing = false;
        Model.CancelEdit();
        OnPropertyChanged(nameof(FullName));
    }
}