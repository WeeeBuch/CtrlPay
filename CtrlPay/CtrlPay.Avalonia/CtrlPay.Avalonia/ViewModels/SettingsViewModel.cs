using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels
{
    internal partial class SettingsViewModel : ViewModelBase
    {
        #region Theme and Translation
        // Seznam všech hodnot z vašeho Enumu
        public IEnumerable<ThemeManager.AppTheme> AvailableThemes { get; } =
            Enum.GetValues(typeof(ThemeManager.AppTheme)).Cast<ThemeManager.AppTheme>();

        [ObservableProperty]
        private ThemeManager.AppTheme _selectedTheme = SettingsManager.Current.Theme;

        public IEnumerable<TranslationManager.AppLanguage> AvailableLanguages { get; } =
            Enum.GetValues(typeof(TranslationManager.AppLanguage)).Cast<TranslationManager.AppLanguage>();

        [ObservableProperty]
        private TranslationManager.AppLanguage _selectedLanguage = SettingsManager.Current.Language;

        partial void OnSelectedThemeChanged(ThemeManager.AppTheme value) => ThemeManager.Apply(value);

        partial void OnSelectedLanguageChanged(TranslationManager.AppLanguage value) => TranslationManager.Apply(value);
        #endregion

        #region Api
        public enum ConnectionStatus { None, Testing, Success, Error }
        [ObservableProperty] private string _apiUrl = SettingsManager.Current.ConnectionString;
        [ObservableProperty] private int _refreshIntervalSeconds = 30; // Výchozí hodnota v sekundách
        [ObservableProperty] private ObservableCollection<string> _recentApiUrls = [.. SettingsManager.Current.SavedConnections];
        [ObservableProperty] private bool _isTestingConnection;
        [ObservableProperty] private bool _isErrorVisible = false;
        [ObservableProperty] private string _statusBoxText = "Untested";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TestStatusText))]
        private ConnectionStatus _testStatus = ConnectionStatus.None;
        public string TestStatusText => _testStatus.ToString();

        partial void OnApiUrlChanged(string value) { ApiUrl = value; SetMassageBoxVisibility(false);}
        partial void OnRefreshIntervalSecondsChanged(int value) => RefreshIntervalSeconds = value;
        private void SetMassageBoxVisibility(bool s) => IsErrorVisible = s;

        private void SaveConnection()
        {
            if (!RecentApiUrls.Contains(ApiUrl))
            {
                if (string.IsNullOrWhiteSpace(ApiUrl)) return;

                RecentApiUrls.Insert(0, ApiUrl);
                if (RecentApiUrls.Count > 5) RecentApiUrls.RemoveAt(5);

                SettingsManager.Current.SavedConnections = [.. RecentApiUrls];
            }
        }

        [RelayCommand]
        private async Task TestConnection()
        {
            TestStatus = ConnectionStatus.Testing; // Spustí loader
            IsTestingConnection = true;
            bool result = false;

            try
            {
                result = await ToDoRepo.TestConnectionToAPI(ApiUrl);
            }
            finally
            {
                IsTestingConnection = false;
                SetMassageBoxVisibility(true);

                if (result)
                {
                    TestStatus = ConnectionStatus.Success; // Zelená
                    SaveConnection();
                    StatusBoxText = TranslationManager.GetString("SettingsView.Status.Succes");
                    await Task.Delay(5000);
                    SetMassageBoxVisibility(false);
                    TestStatus = ConnectionStatus.None; // Reset barvy
                }
                else
                {
                    TestStatus = ConnectionStatus.Error; // Červená
                    StatusBoxText = TranslationManager.GetString("SettingsView.Status.Error");
                }
            }
        }
        #endregion
    }
}
