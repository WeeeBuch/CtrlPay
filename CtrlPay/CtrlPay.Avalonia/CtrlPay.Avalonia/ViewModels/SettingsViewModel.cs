using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels
{
    internal partial class SettingsViewModel : ViewModelBase
    {
        // Seznam všech hodnot z vašeho Enumu
        public IEnumerable<ThemeManager.AppTheme> AvailableThemes { get; } =
            Enum.GetValues(typeof(ThemeManager.AppTheme)).Cast<ThemeManager.AppTheme>();

        [ObservableProperty]
        private ThemeManager.AppTheme _selectedTheme = SettingsManager.Current.Theme;

        public IEnumerable<TranslationManager.AppLanguage> AvailableLanguages { get; } =
            Enum.GetValues(typeof(TranslationManager.AppLanguage)).Cast<TranslationManager.AppLanguage>();

        [ObservableProperty]
        private TranslationManager.AppLanguage _selectedLanguage = SettingsManager.Current.Language;

        // Logika se spustí při změně SelectedTheme
        partial void OnSelectedThemeChanged(ThemeManager.AppTheme value)
        {
            ThemeManager.Apply(value);
        }

        // Logika se spustí při změně SelectedLanguage
        partial void OnSelectedLanguageChanged(TranslationManager.AppLanguage value)
        {
            TranslationManager.Apply(value);
        }

        [ObservableProperty]
        private string _apiUrl = SettingsManager.Current.ConnectionString; 

        [ObservableProperty]
        private int _refreshIntervalSeconds = 30; // Výchozí hodnota v sekundách

        [ObservableProperty]
        private ObservableCollection<string> _recentApiUrls = new()
        {
            "https://api.ctrlpay.cz/v1",
            "https://dev.ctrlpay.cz/api"
        };

        partial void OnApiUrlChanged(string value)
        {
            ApiUrl = value;
            //SettingsManager.Current.ConnectionString = value;
        }

        partial void OnRefreshIntervalSecondsChanged(int value)
        {
            RefreshIntervalSeconds = value;
        }

        [RelayCommand]
        private void SaveConnection()
        {
            if (!RecentApiUrls.Contains(ApiUrl))
            {
                if (string.IsNullOrWhiteSpace(ApiUrl)) return;

                RecentApiUrls.Insert(0, ApiUrl);
                // Omezíme historii např. na 3 položky
                if (RecentApiUrls.Count > 3) RecentApiUrls.RemoveAt(3);
            }
            // Logika pro trvalé uložení do SettingsManager
        }

        [RelayCommand]
        private async Task TestConnection()
        {
            // Zde bude logika pro testování (např. HTTP GET na /health)
            // Můžete nastavit nějakou property "IsConnecting" pro zobrazení loaderu
        }
    }
}
