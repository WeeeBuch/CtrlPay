using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos.Frontend;

namespace CtrlPay.Avalonia.ViewModels;

/// <summary>
/// Zpráva odesílaná při kliknutí na kartu pro filtrování transakcí.
/// </summary>
public record NavigationFilterMessage(StatusEnum Filter);

public partial class DashboardTileViewModel : ViewModelBase
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _titleKey = string.Empty;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private int _count;
    [ObservableProperty] private string _icon = string.Empty;
    [ObservableProperty] private string _color = "#FFFFFF";
    [ObservableProperty] private StatusEnum _filterStatus;

    public DashboardTileViewModel()
    {
        // Registrace pro změnu jazyka, aby se nadpis karty mohl přeložit za běhu
        TranslationManager.LanguageChanged.Add(UpdateTitle);
    }

    private void UpdateTitle()
    {
        if (!string.IsNullOrEmpty(TitleKey))
            Title = TranslationManager.GetString(TitleKey);
    }

    public void GiveTitleKey(string key)
    {
        TitleKey = key;
        UpdateTitle();
    }

    [RelayCommand]
    private void Click()
    {
        // Odešleme zprávu o tom, že uživatel chce vidět transakce s tímto filtrem
        WeakReferenceMessenger.Default.Send(new NavigationFilterMessage(FilterStatus));
    }
}
