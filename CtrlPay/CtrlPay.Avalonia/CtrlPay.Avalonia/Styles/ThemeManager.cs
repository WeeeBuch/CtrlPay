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
            Lime,
            Blue,
            DarkGreen,
            DarkOrange,
            Green,
            LightBlue,
            LightOrange,
            Orange,
            Pink,
            Purple,
            Red,
            Tourquise
        }

        public static void Apply(AppTheme theme)
        {
            var resources = Application.Current!.Resources;

            resources.MergedDictionaries.Clear();

            var uri = GetUri(theme);

            resources.MergedDictionaries.Add(new ResourceInclude(uri)
            {
                Source = uri
            });
        }

        public static Uri GetUri(AppTheme theme) => theme switch
        {
            AppTheme.Lime => new Uri("avares://CtrlPay.Avalonia/Styles/Lime.axaml"),
            AppTheme.Blue => new Uri("avares://CtrlPay.Avalonia/Styles/Blue.axaml"),
            AppTheme.DarkGreen => new Uri("avares://CtrlPay.Avalonia/Styles/DarkGreen.axaml"),
            AppTheme.DarkOrange => new Uri("avares://CtrlPay.Avalonia/Styles/DarkOrange.axaml"),
            AppTheme.Green => new Uri("avares://CtrlPay.Avalonia/Styles/Green.axaml"),
            AppTheme.LightBlue => new Uri("avares://CtrlPay.Avalonia/Styles/LightBlue.axaml"),
            AppTheme.LightOrange => new Uri("avares://CtrlPay.Avalonia/Styles/LightOrange.axaml"),
            AppTheme.Orange => new Uri("avares://CtrlPay.Avalonia/Styles/Orange.axaml"),
            AppTheme.Pink => new Uri("avares://CtrlPay.Avalonia/Styles/Pink.axaml"),
            AppTheme.Purple => new Uri("avares://CtrlPay.Avalonia/Styles/Purple.axaml"),
            AppTheme.Red => new Uri("avares://CtrlPay.Avalonia/Styles/Red.axaml"),
            AppTheme.Tourquise => new Uri("avares://CtrlPay.Avalonia/Styles/Turquoise.axaml"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
