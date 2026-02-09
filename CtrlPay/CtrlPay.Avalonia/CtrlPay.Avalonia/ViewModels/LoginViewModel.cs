using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Avalonia.Views;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
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

    // --- API adresa ---
    private bool hasAPI;

    [RelayCommand]
    private async Task Register()
    {
        AppLogger.Info($"Registering...");
        ReturnModel<bool> returnModel = await AuthRepo.Register(RegUsername, RegCode, RegPassword, RegConfirmPassword);
        bool success = returnModel.Body;

        if (success) 
        {
            AppLogger.Info($"Register succesfull, closing Login window and starting Main window...");
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            AppLogger.Error("Failed to Register", returnModel);
            Message = TranslationManager.GetErrorCode(returnModel);
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        AppLogger.Info($"Logining....");

        if (!hasAPI)
        {
            AppLogger.Warning("Settings does not have API connection. Summoning API connect window...");
            var vm = new APIConnectViewModel();
            var window = new ConnectAPIWindow { DataContext = vm };
            // TADY CHYBĚLO:
            vm.CloseAction = () => window.Close();

            var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var owner = desktop?.MainWindow;

            if (owner != null)
            {
                await window.ShowDialog(owner);
            }
        }

        ReturnModel<bool> returnModel = await AuthRepo.Login(Username, Password, default);
        bool success = returnModel.Body;

        if (success)
        {
            AppLogger.Info($"Loged in succesfully, closing Login window and starting Main window...");
            _navigation.ShowMainWindow();
            _navigation.CloseLogin();
        }
        else
        {
            AppLogger.Error("Failed to Login", returnModel);
            Message = TranslationManager.GetErrorCode(returnModel);
        }
    }

    [ObservableProperty] public string message = "";
    private readonly INavigationService _navigation;

    public LoginViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        hasAPI = Credentials.BaseUri != "";
    }
}
