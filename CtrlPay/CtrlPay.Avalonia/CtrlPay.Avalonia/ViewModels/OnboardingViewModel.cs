using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class OnboardingViewModel : ViewModelBase
{
    [ObservableProperty] private int _currentStep = 0;

    // --- Krok 1: Jazyk ---
    public IEnumerable<TranslationManager.AppLanguage> AvailableLanguages { get; } =
        Enum.GetValues(typeof(TranslationManager.AppLanguage)).Cast<TranslationManager.AppLanguage>();

    [ObservableProperty] private TranslationManager.AppLanguage _selectedLanguage = SettingsManager.Current.Language;

    partial void OnSelectedLanguageChanged(TranslationManager.AppLanguage value) => TranslationManager.Apply(value);

    // --- Krok 2: Téma ---
    public IEnumerable<ThemeManager.AppTheme> AvailableThemes { get; } =
        Enum.GetValues(typeof(ThemeManager.AppTheme)).Cast<ThemeManager.AppTheme>();

    [ObservableProperty] private ThemeManager.AppTheme _selectedTheme = SettingsManager.Current.Theme;

    partial void OnSelectedThemeChanged(ThemeManager.AppTheme value) => ThemeManager.Apply(value);

    // --- Krok 3: API ---
    [ObservableProperty] private string _apiUrl = "http://";
    [ObservableProperty] private bool _isTestingConnection;
    [ObservableProperty] private bool _isSuccessVisible;
    [ObservableProperty] private bool _isErrorVisible;
    [ObservableProperty] private string _statusBoxText = "";

    // Navigační pomocníci
    public bool IsNotLastStep => CurrentStep < 2;
    public bool IsLastStep => CurrentStep == 2;

    [RelayCommand]
    private void Back()
    {
        if (CurrentStep > 0)
        {
            CurrentStep--;
            UpdateNavigationProperties();
        }
    }

    public bool CanGoBack => CurrentStep > 0;

    [RelayCommand]
    private void Next()
    {
        if (CurrentStep < 2)
        {
            CurrentStep++;
            UpdateNavigationProperties();
        }
    }

    private void UpdateNavigationProperties()
    {
        OnPropertyChanged(nameof(IsNotLastStep));
        OnPropertyChanged(nameof(IsLastStep));
        OnPropertyChanged(nameof(CanGoBack));
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        IsTestingConnection = true;
        IsErrorVisible = false;
        IsSuccessVisible = false;

        try
        {
            var result = await ToDoRepo.TestConnectionToAPI(ApiUrl);
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
        finally { IsTestingConnection = false; }
    }

    [RelayCommand(CanExecute = nameof(IsSuccessVisible))]
    private void Finish()
    {
        // Uložit finální stav
        SettingsManager.Current.Language = SelectedLanguage;
        SettingsManager.Current.Theme = SelectedTheme;
        SettingsManager.Current.ConnectionString = ApiUrl;
        SettingsManager.Save(SettingsManager.Current);

        WeakReferenceMessenger.Default.Send(new OnboardingFinishedMessage());
    }
}