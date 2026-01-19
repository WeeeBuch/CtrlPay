using Avalonia.Data.Converters;
using Avalonia.Media;
using CtrlPay.Avalonia.Styles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.Converters
{
    public class ThemeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ThemeManager.AppTheme theme)
            {
                // Zde zadejte Accent barvy z vašich axaml souborů
                string hexColor = theme switch
                {
                    ThemeManager.AppTheme.Red => "#63080a",
                    ThemeManager.AppTheme.Blue => "#0a3b77",
                    ThemeManager.AppTheme.Lime => "#26841d",
                    ThemeManager.AppTheme.DarkGreen => "#15351d",
                    ThemeManager.AppTheme.DarkOrange => "#592c14",
                    ThemeManager.AppTheme.Green => "#296633",
                    ThemeManager.AppTheme.LightBlue => "#2470b4",
                    ThemeManager.AppTheme.LightOrange => "#d55a0b",
                    ThemeManager.AppTheme.Orange => "#b85b23",
                    ThemeManager.AppTheme.Pink => "#e71a60",
                    ThemeManager.AppTheme.Purple => "#5a1386",
                    ThemeManager.AppTheme.Tourquise => "#126967",
                    _ => "#808080"
                };

                return Color.Parse(hexColor);
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
