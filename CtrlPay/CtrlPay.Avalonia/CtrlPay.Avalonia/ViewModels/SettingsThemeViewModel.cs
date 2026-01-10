using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels
{
    internal partial class SettingsThemeViewModel : ViewModelBase
    {
        // Seznam všech hodnot z vašeho Enumu
        public IEnumerable<ThemeManager.AppTheme> AvailableThemes { get; } =
            Enum.GetValues(typeof(ThemeManager.AppTheme)).Cast<ThemeManager.AppTheme>();

        [ObservableProperty]
        private ThemeManager.AppTheme _selectedTheme;

        // Logika se spustí při změně SelectedTheme
        partial void OnSelectedThemeChanged(ThemeManager.AppTheme value)
        {
            ThemeManager.Apply(value);
        }
    }
}
