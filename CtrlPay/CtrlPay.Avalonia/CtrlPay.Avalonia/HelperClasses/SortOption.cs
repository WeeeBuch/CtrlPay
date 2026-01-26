using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.HelperClasses;

public partial class SortOption : ObservableObject
{
    public string Key { get; set; }

    [ObservableProperty]
    private string? displayName;

    public string DisplayNameKay;

    public SortOption(string key, string displayNameKey)
    {
        Key = key;
        DisplayNameKay = displayNameKey;

        UpdateDisplayName();

        TranslationManager.LanguageChanged.Add(UpdateDisplayName);
    }

    private void UpdateDisplayName()
    {
        if (!string.IsNullOrEmpty(DisplayNameKay))
            DisplayName = TranslationManager.GetString(DisplayNameKay);
    }
}
