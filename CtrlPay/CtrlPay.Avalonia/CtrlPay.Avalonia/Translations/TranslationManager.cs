using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using static CtrlPay.Avalonia.Styles.ThemeManager;

namespace CtrlPay.Avalonia.Translations
{
    internal static class TranslationManager
    {
        public enum AppLanguage
        {
            Base,
            English
        }

        private static ResourceInclude? _translationResources;
        public static List<Action> LanguageChanged = [];

        public static void Apply(AppLanguage language)
        {
            var resources = Application.Current!.Resources;

            if (_translationResources != null)
                resources.MergedDictionaries.Remove(_translationResources);

            var uri = GetUri(language);

            _translationResources = new ResourceInclude(uri)
            {
                Source = uri
            };

            resources.MergedDictionaries.Add(_translationResources);
            foreach (var action in LanguageChanged.ToList())
            {
                action();
            }
        }

        public static Uri GetUri(AppLanguage language) => language switch
        {
            AppLanguage.English => new Uri("avares://CtrlPay.Avalonia/Translations/English.axaml"),
            AppLanguage.Base => new Uri("avares://CtrlPay.Avalonia/Translations/TranslationBase.axaml"),
            _ => new Uri("avares://CtrlPay.Avalonia/Translations/TranslationBase.axaml")
        };

        public static string GetString(string key)
        {
            if (Application.Current!.TryFindResource(key, out var message))
            {
                if (message == null)
                    return "Nah some error happend";

                return message.ToString();
            }
            return "Nah some error happend";
        }

    }
}
