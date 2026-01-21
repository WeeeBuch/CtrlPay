using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Views;
using CtrlPay.Repos;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    // --- Přepínání Přihlásit / Registrovat ---
    [ObservableProperty]
    private bool isLoginSelected = true;

    [RelayCommand]
    private void ToggleLogin()
    {
        IsLoginSelected = true;
    }

    [RelayCommand]
    private void ToggleRegister()
    {
        IsLoginSelected = false;
    }

    // --- Přihlašovací pole ---
    [ObservableProperty]
    private string? username;

    [ObservableProperty]
    private string? password;


    // --- Registrační pole ---
    [ObservableProperty]
    private string? regUsername;

    [ObservableProperty]
    private string? regCode;

    [ObservableProperty]
    private string? regPassword;

    [ObservableProperty]
    private string? regConfirmPassword;

    [RelayCommand]
    private void Register()
    {
        // Předělat chybovou hlášku
        bool succes = AuthRepo.Register(RegUsername, RegCode, RegPassword, RegConfirmPassword);

        if (succes) 
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {// Předělat chybovou hlášku
            Message = AuthRepo.RegisterFailedMessage();
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // Předělat chybovou hlášku
        CancellationToken cancellationToken = new CancellationToken();
        bool success = await AuthRepo.Login(Username, Password, cancellationToken);

        if (success) 
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            // Předělat chybovou hlášku
            Message = AuthRepo.LoginFailedMessage();
        }
    }

    [ObservableProperty]
    public string message = "";

    private readonly INavigationService _navigation;

    public LoginViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }
}
