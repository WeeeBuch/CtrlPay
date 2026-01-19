using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Linq;

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

    }
}
