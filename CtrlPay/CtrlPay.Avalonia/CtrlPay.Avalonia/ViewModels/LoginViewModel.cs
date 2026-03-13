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

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? apiUrl;

    // --- API adresa ---
    private bool hasAPI;

    [RelayCommand]
    private async Task OpenApiSettings()
    {
        AppLogger.Info("Opening API connect settings...");

        var lifetime = Application.Current?.ApplicationLifetime;

        // Desktop / multi-window behavior unchanged
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new APIConnectViewModel();
            var window = new ConnectAPIWindow { DataContext = vm };
            vm.CloseAction = () => window.Close();

            var owner = desktop.MainWindow;

            if (owner != null)
            {
                await window.ShowDialog(owner);
            }
            else
            {
                window.Show();
            }
        }
        else
        {
            // On mobile / web we currently don't support a separate window.
            AppLogger.Warning("API connect window is only available on desktop.");
        }

        // After closing / attempting settings, refresh API info
        ApiUrl = Credentials.BaseUri;
        hasAPI = !string.IsNullOrEmpty(ApiUrl);
    }

    [RelayCommand]
    private async Task Register()
    {
        if (!hasAPI)
        {
            await OpenApiSettings();
            if (!hasAPI) return;
        }

        IsBusy = true;
        try
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
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (!hasAPI)
        {
            AppLogger.Warning("Settings does not have API connection. Summoning API connect window...");
            await OpenApiSettings();
            if (!hasAPI) return;
        }

        IsBusy = true;
        try
        {
            AppLogger.Info($"Logining....");
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
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty] public string message = "";
    private readonly INavigationService _navigation;

    public LoginViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        ApiUrl = Credentials.BaseUri;
        hasAPI = !string.IsNullOrEmpty(ApiUrl);
    }
}
