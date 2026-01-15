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
    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string? titleKey;

    // Tato vlastnost se automaticky aktualizuje, když se změní TitleKey
    [ObservableProperty]
    private string? title;

    public CounterPieceModel()
    {
        // Pozor: V produkční aplikaci by bylo lepší použít WeakReference 
        // nebo zajistit IDisposable pro odhlášení z akce.
        TranslationManager.LanguageChanged.Add(UpdateTitle);
    }

    private void UpdateTitle()
    {
        if (!string.IsNullOrEmpty(TitleKey))
        {
            Title = TranslationManager.GetString(TitleKey);
        }
    }

    public void GiveTitleKey(string key)
    {
        TitleKey = key;
        UpdateTitle(); // Aktualizujeme text hned při nastavení klíče
    }
}