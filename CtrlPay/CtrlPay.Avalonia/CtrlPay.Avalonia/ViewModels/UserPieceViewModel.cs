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
    
    public Role[] RolesList => Enum.GetValues<Role>();

    public UserPieceViewModel(FrontendUserDTO model)
    {
        _model = model;
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
        
        await AdminRepo.UpdateAdminUser(Model);
        UpdateHandler.HandleUpdatedAdminUsers();
    }
}
