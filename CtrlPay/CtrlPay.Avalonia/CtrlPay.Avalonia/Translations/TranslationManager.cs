using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Entities;
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
            English,
            Czech,
            German,
            French,
            Japanese,
            Mandarin,
            Russian,
            Spanish,
            Polish,
            Slovak,
            Moravian,
            Pirate,
            Leetspeak
        }

        private static ResourceInclude? _translationResources;
        public static List<Action> LanguageChanged = [];
        public static AppLanguage CurrentLanguage { get; private set; }

        public static void Apply(AppLanguage language)
        {
            CurrentLanguage = language;
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

            SettingsManager.Current.Language = language;
        }

        public static Uri GetUri(AppLanguage language) => language switch
        {
            AppLanguage.English => new Uri("avares://CtrlPay.Avalonia/Translations/English.axaml"),
            AppLanguage.Base => new Uri("avares://CtrlPay.Avalonia/Translations/TranslationBase.axaml"),
            AppLanguage.Czech => new Uri("avares://CtrlPay.Avalonia/Translations/Czech.axaml"),
            AppLanguage.German => new Uri("avares://CtrlPay.Avalonia/Translations/German.axaml"),
            AppLanguage.French => new Uri("avares://CtrlPay.Avalonia/Translations/French.axaml"),
            AppLanguage.Japanese => new Uri("avares://CtrlPay.Avalonia/Translations/Japanese.axaml"),
            AppLanguage.Mandarin => new Uri("avares://CtrlPay.Avalonia/Translations/Mandarin.axaml"),
            AppLanguage.Russian => new Uri("avares://CtrlPay.Avalonia/Translations/Russian.axaml"),
            AppLanguage.Spanish => new Uri("avares://CtrlPay.Avalonia/Translations/Spanish.axaml"),
            AppLanguage.Polish => new Uri("avares://CtrlPay.Avalonia/Translations/Polish.axaml"),
            AppLanguage.Slovak => new Uri("avares://CtrlPay.Avalonia/Translations/Slovak.axaml"),
            AppLanguage.Moravian => new Uri("avares://CtrlPay.Avalonia/Translations/Moravian.axaml"),
            AppLanguage.Pirate => new Uri("avares://CtrlPay.Avalonia/Translations/Pirate.axaml"),
            AppLanguage.Leetspeak => new Uri("avares://CtrlPay.Avalonia/Translations/Leet.axaml"),
            _ => new Uri("avares://CtrlPay.Avalonia/Translations/TranslationBase.axaml")
        };

        public static string GetString(string key)
        {
            if (Application.Current!.TryFindResource(key, out var message))
            {
                if (message == null)
                    return "Nah Message is null";

                return message.ToString();
            }
            return $"Nah You forgot to implement this in {CurrentLanguage} translation";
        }

        public static string GetErrorCode(ReturnModel returnModel)
        {
            string s = $"ErrorCode.{returnModel.ReturnCode}";
            if (Application.Current!.TryFindResource(s, out var message))
            {
                if (message == null)
                    return "Nah Message is null";

                return message.ToString();
            }

            return $"Not implemented showing base message: {returnModel.BaseMessage}";
        }

    }
}
