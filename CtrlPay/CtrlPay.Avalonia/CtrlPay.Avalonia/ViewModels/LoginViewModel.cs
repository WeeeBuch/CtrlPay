using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Views;
using CtrlPay.Repos;
using System;
using System.ComponentModel;

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
    private string username;

    [ObservableProperty]
    private string password;


    // --- Registrační pole ---
    [ObservableProperty]
    private string regUsername;

    [ObservableProperty]
    private string regEmail;

    [ObservableProperty]
    private string regPassword;

    [ObservableProperty]
    private string regConfirmPassword;

    [RelayCommand]
    private void Register()
    {
        bool succes = Repo.Register(RegUsername, RegEmail, RegPassword, RegConfirmPassword);

        if (succes) 
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            Message = Repo.RegisterFailedMessage();
        }
    }

    [RelayCommand]
    private void Login()
    {
        bool succes = Repo.Login(Username, Password);

        if (succes) 
        {
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            Message = Repo.LoginFailedMessage();
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
