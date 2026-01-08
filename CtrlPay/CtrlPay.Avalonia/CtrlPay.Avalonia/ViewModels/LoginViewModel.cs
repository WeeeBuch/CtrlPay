using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        Title = $"Register clicked! Username: {RegUsername}, Email: {RegEmail}";
    }

    [RelayCommand]
    private void Login()
    {
        Title = $"Login clicked! Username: {Username}";
    }

    [ObservableProperty]
    private string title = "Welcome to Avalonia!";
}
