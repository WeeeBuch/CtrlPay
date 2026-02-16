using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System.Reflection;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CustomerPieceViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))] // Automaticky aktualizuje jméno v hlavičce
    public FrontendCustomerDTO _model;
    [ObservableProperty] private bool _isExpanded = false;

    [ObservableProperty] private bool _editing = false;

    public CustomerPieceViewModel(FrontendCustomerDTO customer)
    {
        _model = customer;
    }

    // Bezpečná vlastnost pro zobrazení jména
    public string DisplayName => Model == null
        ? "Načítám..."
        : (Model.Physical
            ? $"{Model.Title} {Model.FirstName} {Model.LastName}".Trim()
            : (string.IsNullOrWhiteSpace(Model.Company) ? "Neznámá firma" : Model.Company));

    // Přidej tyto vlastnosti do CustomerPieceViewModel
    public bool IsPhysical => Model?.Physical ?? false;
    public bool IsCompany => !IsPhysical;


    [RelayCommand]
    public void TogglePh()
    {
        Model.Physical = IsPhysical;
        RefreshUI();
    }

    public void RefreshUI()
    {
        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(IsPhysical)); // Tohle probudí StackPanel pro jméno
        OnPropertyChanged(nameof(IsCompany));  // Tohle probudí StackPanel pro firmu
    }

    [RelayCommand]
    public void StartEdit()
    {
        Model.BeginEdit();
        Editing = true;
        IsExpanded = true;
    }

    [RelayCommand]
    public async Task EndEditAsync()
    {
        // Nejdřív vypneme editing, aby SyncCollections mohl model případně aktualizovat
        Editing = false;
        Model.EndEdit();

        await CustomerRepo.UpdateCustomer(Model);

        // Důležité: Refreshneme UI a oznámíme globální změnu
        RefreshUI();
        UpdateHandler.HandleUpdatedCustomers();
    }

    [RelayCommand]
    public void CancelEdit()
    {
        Editing = false;
        Model.CancelEdit();
        RefreshUI();
    }

    [RelayCommand]
    public async Task DeleteEditCommand()
    {
        Editing = false;
        await CustomerRepo.DeleteCustomer(Model);
        UpdateHandler.HandleUpdatedCustomers();
    }
}