using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CounterPieceModel : ViewModelBase
{
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private string? titleKey;
    [ObservableProperty] private string? title;
    [ObservableProperty] private bool hasButton;
    [ObservableProperty] private string? buttonKey;
    [ObservableProperty] private string? buttonText;

    public CounterPieceModel()
    {
        HasButton = false;
        TranslationManager.LanguageChanged.Add(UpdateTitle);
    }

    private void UpdateTitle()
    {
        if (!string.IsNullOrEmpty(TitleKey))
        {
            Title = TranslationManager.GetString(TitleKey);
        }

        if (HasButton && !string.IsNullOrEmpty(ButtonKey))
        {
            ButtonText = TranslationManager.GetString(ButtonKey);
        }
    }

    public void GiveTitleKey(string key)
    {
        TitleKey = key;
        UpdateTitle();
    }
}