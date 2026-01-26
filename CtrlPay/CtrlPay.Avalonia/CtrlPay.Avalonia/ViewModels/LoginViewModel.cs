using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Avalonia.Views;
using CtrlPay.Entities;
using CtrlPay.Repos;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    // --- Přepínání Přihlásit / Registrovat ---
    [ObservableProperty] private bool isLoginSelected = true;

    [RelayCommand] 
    private void ToggleLogin() => IsLoginSelected = true;

    [RelayCommand]
    private void ToggleRegister() => IsLoginSelected = false;

    // --- Přihlašovací pole ---
    [ObservableProperty] private string? username;
    [ObservableProperty] private string? password;

    // --- Registrační pole ---
    [ObservableProperty] private string? regUsername;
    [ObservableProperty] private string? regCode;
    [ObservableProperty] private string? regPassword;
    [ObservableProperty] private string? regConfirmPassword;
   

    [RelayCommand]
    private async Task Register()
    {
        ReturnModel<bool> returnModel = await AuthRepo.Register(RegUsername, RegCode, RegPassword, RegConfirmPassword);
        bool success = returnModel.Body;

        if (success) 
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            Message = TranslationManager.GetErrorCode(returnModel);
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ReturnModel<bool> returnModel = await AuthRepo.Login(Username, Password, default);
        bool success = returnModel.Body;

        if (success)
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            Message = TranslationManager.GetErrorCode(returnModel);
        }
    }

    [ObservableProperty] public string message = "";
    private readonly INavigationService _navigation;

    public LoginViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }
}
