using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CtrlPay.Avalonia.Styles
{
    internal class ThemeItem
    {
        public ThemeManager.AppTheme Theme { get; }
        public IBrush PreviewBrush { get; }
        public ICommand ApplyThemeCommand { get; }

        public ThemeItem(ThemeManager.AppTheme theme)
        {
            Theme = theme;

            var uri = ThemeManager.GetUri(theme);

            var dict = new ResourceInclude(uri)
            {
                Source = uri
            };

            // Oprava: ResourceInclude nepodporuje indexování [].
            // Místo toho použijte TryGetResource pro získání hodnoty.
            if (dict.TryGetResource("Brush.Accent", null, out var brushObj) && brushObj is IBrush brush)
            {
                PreviewBrush = brush;
            }
            else
            {
                PreviewBrush = null!; // nebo nastavte výchozí hodnotu, pokud je potřeba
            }

            ApplyThemeCommand = new RelayCommand(() =>
            {
                ThemeManager.Apply(theme);
            });
        }
    }
}
