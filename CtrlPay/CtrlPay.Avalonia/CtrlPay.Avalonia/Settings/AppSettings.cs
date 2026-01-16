using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.Settings
{
    internal class AppSettings
    {
        private TranslationManager.AppLanguage _language = TranslationManager.AppLanguage.Base;
        public TranslationManager.AppLanguage Language
        {
            get => _language;
            set { _language = value; Save(); }
        }

        private ThemeManager.AppTheme _theme = ThemeManager.AppTheme.Lime;
        public ThemeManager.AppTheme Theme
        {
            get => _theme;
            set { _theme = value; Save(); }
        }

        // Pomocná metoda pro uložení sebe sama
        private void Save()
        {
            SettingsManager.Save(this);
        }
    }
}
