using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class UserPieceViewModel : ViewModelBase
{
    [ObservableProperty] private FrontendUserDTO _model;
    [ObservableProperty] private bool _editing = false;
    
    public static Role[] RolesList => Enum.GetValues<Role>();

    // Seznam zákazníků z repozitáře
    public List<FrontendCustomerDTO> Customers => CustomerRepo.GetCustomers();

    // Vlastnost pro snadné nabindování vybraného zákazníka v ComboBoxu
    public FrontendCustomerDTO? SelectedCustomer
    {
        get => Customers.FirstOrDefault(c => c.Id == Model.CustomerId);
        set
        {
            Model.CustomerId = value?.Id;
            OnPropertyChanged(nameof(SelectedCustomer));
        }
    }

    // Pomocná vlastnost pro zobrazení jména zákazníka v needitačním režimu
    public string CustomerDisplayName => Model.CustomerFullName ?? "---";

    // Viditelnost výběru zákazníka (pouze pro roli Customer)
    public bool ShowCustomerSelection => Model.Role == Role.Customer;

    public UserPieceViewModel(FrontendUserDTO model)
    {
        _model = model;
        // Přidáme listener na změnu vlastností v modelu, pokud implementuje INotifyPropertyChanged
        // Model FrontendUserDTO zatím neimplementuje INotifyPropertyChanged pro Role, 
        // ale my můžeme hlídat celou změnu Modelu.
    }

    // Musíme hlídat změny v modelu, aby UI vědělo o ShowCustomerSelection
    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Model))
        {
            OnPropertyChanged(nameof(ShowCustomerSelection));
            OnPropertyChanged(nameof(SelectedCustomer));
            OnPropertyChanged(nameof(CustomerDisplayName));
        }
    }

    // Tuhle metodu zavoláme z XAML přes EventTrigger nebo ji budeme volat manuálně při změně Role
    public void RefreshRoleVisibility()
    {
        OnPropertyChanged(nameof(ShowCustomerSelection));
    }

    [RelayCommand]
    public void StartEdit()
    {
        Model.BeginEdit();
        Editing = true;
    }

    [RelayCommand]
    public void CancelEdit()
    {
        Model.CancelEdit();
        Editing = false;
        
        if (Model.Id == 0)
        {
            // Pokud to byl nový uživatel, signál k refreshnutí celého seznamu (aby zmizel)
            UpdateHandler.HandleUpdatedAdminUsers();
        }
    }

    [RelayCommand]
    public async Task Save()
    {
        Editing = false;
        Model.EndEdit();
        
        await AdminRepo.UpdateUser(Model);
        UpdateHandler.HandleUpdatedAdminUsers();
    }
}
