using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Avalonia.Views;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
            _navigation.NavigateToMain();
        }
        else
        {
            AppLogger.Error("Failed to Register", returnModel);
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        Credentials.BaseUri = "https://www.action-games.cz/";


        ReturnModel<bool> returnModel = await AuthRepo.Login(Username, Password, default);
        bool success = returnModel.Body;
        Message = $"Body={returnModel.Body} Code={returnModel.ReturnCode}";

        if (success)
        {
            _navigation.NavigateToMain();
        }
        else
        {
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
