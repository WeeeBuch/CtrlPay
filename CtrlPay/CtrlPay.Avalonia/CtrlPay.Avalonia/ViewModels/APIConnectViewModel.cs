using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using ShimSkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class APIConnectViewModel : ViewModelBase
{
    [ObservableProperty] private string _apiUrl = "http://";
    [ObservableProperty] private bool _isTestingConnection = false;
    [ObservableProperty] private bool _isSuccessVisible = false;
    [ObservableProperty] private bool _isErrorVisible = false;
    [ObservableProperty] private string _statusBoxText = "";

    [RelayCommand]
    private async Task TestConnection()
    {
        IsTestingConnection = true;
        IsErrorVisible = false;
        IsSuccessVisible = false;

        FinishCommand.NotifyCanExecuteChanged();

        try
        {
            var result = await HealthRepo.TestConnectionToAPI(ApiUrl);
            if (result)
            {
                IsSuccessVisible = true;
                StatusBoxText = TranslationManager.GetString("SettingsView.Status.Succes");
            }
            else
            {
                IsErrorVisible = true;
                StatusBoxText = TranslationManager.GetString("SettingsView.Status.Error");
            }
        }
        finally
        {
            IsTestingConnection = false;
            FinishCommand.NotifyCanExecuteChanged();
        }
    }

    public Action? CloseAction;

    [RelayCommand(CanExecute = nameof(IsSuccessVisible))]
    public void Finish()
    {
        SettingsManager.Current.ConnectionString = ApiUrl;
        CloseAction?.Invoke();
    }
}
