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

        // Připojení aplikace k API
        private string _connectionString = string.Empty;
        public string ConnectionString
        {
            get => _connectionString;
            set { _connectionString = value; Save(); }
        }

        // Uložená připojení
        private List<string> _savedConnections = ["127.0.0.1"];
        public List<string> SavedConnections
        {
            get => _savedConnections;
            set { _savedConnections = value; Save(); }
        }

        // Frekvence kontroly nových dat
        private int _refreshRate = 30;
        public int RefreshRate
        {
            get => _refreshRate;
            set { _refreshRate = value; Save(); }
        }


        // Pomocná metoda pro uložení sebe sama
        private void Save()
        {
            SettingsManager.Save(this);
        }
    }
}
