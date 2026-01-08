using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using System;
using System.Collections.Generic;

namespace CtrlPay.Avalonia.Styles
{
    internal static class ThemeManager
    {
        public enum AppTheme
        {
            Dark,
            DarkGreen
        }

        public static void Apply(AppTheme theme)
        {
            var resources = Application.Current!.Resources;

            resources.MergedDictionaries.Clear();

            var uri = theme switch
            {
                AppTheme.Dark => new Uri("avares://CtrlPay.Avalonia/Styles/DarkColors.axaml"),
                AppTheme.DarkGreen => new Uri("avares://CtrlPay.Avalonia/Styles/DarkGreenColors.axaml"),
                _ => throw new ArgumentOutOfRangeException()
            };

            ResourceInclude resourcer = new(uri)
            {
                Source = uri
            };

            resources.MergedDictionaries.Add(resourcer);
        }
    }
}
