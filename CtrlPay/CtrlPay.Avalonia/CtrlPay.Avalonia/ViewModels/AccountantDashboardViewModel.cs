using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Avalonia.ViewModels;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class AccountantDashboardViewModel : ViewModelBase
{
    // Data
    private AccountantChartDataDTO? _lastChartData;

    // Grafy
    [ObservableProperty] private ISeries[] _incomeSeries;
    [ObservableProperty] private Axis[] _xAxes;
    [ObservableProperty] private ISeries[] _statusSeries;
    [ObservableProperty] private SolidColorPaint? _tooltipBackgroundPaint;
    [ObservableProperty] private SolidColorPaint? _tooltipTextPaint;

    // Fixní instance sérií pro stabilitu barev a animací
    private readonly LineSeries<decimal> _incomeLineSeries = new()
    {
        GeometrySize = 10,
        LineSmoothness = 1,
        Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 3 },
        Fill = new SolidColorPaint(SKColors.CornflowerBlue.WithAlpha(40)),
        GeometryFill = new SolidColorPaint(SKColors.CornflowerBlue),
        GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 2 }
    };

    // Kostky
    [ObservableProperty] private DashboardTileViewModel _overpaidTile;
    [ObservableProperty] private DashboardTileViewModel _overdueTile;
    [ObservableProperty] private DashboardTileViewModel _partialTile;
    [ObservableProperty] private DashboardTileViewModel _waitingTile;

    public AccountantDashboardViewModel()
    {
        // Inicializace kostek - nastavíme jim styl a typ filtru
        _overpaidTile = new DashboardTileViewModel
        {
            Color = "#4488FF", // Modrá
            Icon = IconData.Cash,
            FilterStatus = StatusEnum.Overpaid
        };
        _overpaidTile.GiveTitleKey("Accountant.Dashboard.Overpaid");

        _overdueTile = new DashboardTileViewModel
        {
            Color = "#FF4444", // Červená
            Icon = IconData.Debt,
            FilterStatus = StatusEnum.Expired
        };
        _overdueTile.GiveTitleKey("Accountant.Dashboard.Overdue");

        _partialTile = new DashboardTileViewModel
        {
            Color = "#FFCC00", // Žlutá/Oranžová
            Icon = IconData.Cash,
            FilterStatus = StatusEnum.PartiallyPaid
        };
        _partialTile.GiveTitleKey("Accountant.Dashboard.PartiallyPaid");

        _waitingTile = new DashboardTileViewModel
        {
            Color = "#AAAAAA", // Šedá
            Icon = IconData.Dashboard,
            FilterStatus = StatusEnum.WaitingForPayment
        };
        _waitingTile.GiveTitleKey("Accountant.Dashboard.Waiting");

        // Nastavíme pole sérií jednou
        IncomeSeries = [_incomeLineSeries];

        LoadData();

        UpdateHandler.UpdatedData.Add(LoadData);
        
        // Přihlášení ke změně jazyka
        TranslationManager.LanguageChanged.Add(RefreshTranslations);
    }

    private void RefreshTranslations()
    {
        if (_lastChartData == null) return;

        // Pokud už série existují a mají stejný počet jako data, jen aktualizujeme jejich vlastnosti (aby se nepřebarvovaly a neblikaly)
        if (StatusSeries != null && StatusSeries.Length == _lastChartData.StatusBreakdown.Count)
        {
            for (int i = 0; i < StatusSeries.Length; i++)
            {
                if (StatusSeries[i] is PieSeries<int> series)
                {
                    var dataPoint = _lastChartData.StatusBreakdown[i];
                    series.Name = TranslationManager.GetString($"Transaction.Status.{dataPoint.Status}");
                    series.Values = [dataPoint.Count];
                    // Fill ponecháváme stejný, aby barvy neblikaly
                }
            }
        }
        else
        {
            // Vytvoříme série úplně znovu (např. při prvním načtení)
            StatusSeries = [.. _lastChartData.StatusBreakdown.Select(x => new PieSeries<int>
            {
                Values = [x.Count],
                Name = TranslationManager.GetString($"Transaction.Status.{x.Status}"),
                Fill = GetColorForStatus(x.Status),
                InnerRadius = 60
            })];
        }
    }

    private SolidColorPaint GetColorForStatus(string status) => status switch
    {
        "Paid" => new SolidColorPaint(SKColors.MediumSeaGreen),
        "Overpaid" => new SolidColorPaint(SKColors.CornflowerBlue),
        "PartiallyPaid" => new SolidColorPaint(SKColors.Orange),
        "Expired" => new SolidColorPaint(SKColors.Crimson),
        _ => new SolidColorPaint(SKColors.Gray)
    };

    private void LoadData()
    {
        // Načteme souhrnná data z ToDoRepo (zatím Mock data)
        var summary = AccountantRepo.GetAccountantDashboardSummary();

        // Rozdistribuujeme data do jednotlivých kostek
        OverpaidTile.Amount = summary.OverpaidAmount;
        OverpaidTile.Count = summary.OverpaidCount;

        OverdueTile.Amount = summary.OverdueAmount;
        OverdueTile.Count = summary.OverdueCount;

        PartialTile.Amount = summary.PartiallyPaidAmount;
        PartialTile.Count = summary.PartiallyPaidCount;

        WaitingTile.Amount = summary.WaitingAmount;
        WaitingTile.Count = summary.WaitingCount;

        // Načteme data pro grafy
        var data = AccountantRepo.GetAccountantChartData();

        // Jednoduchá kontrola změn obsahu (protože mock vrací vždy novou instanci objektu)
        bool dataChanged = _lastChartData == null || 
                           _lastChartData.StatusBreakdown.Count != data.StatusBreakdown.Count ||
                           !_lastChartData.StatusBreakdown.SequenceEqual(data.StatusBreakdown) ||
                           !_lastChartData.IncomeHistory.Select(x => x.Amount).SequenceEqual(data.IncomeHistory.Select(x => x.Amount));

        _lastChartData = data;

        // Získáme barvy z aplikace pro aktuální téma
        var accentColor = SKColors.CornflowerBlue; // Fallback
        var surfaceColor = SKColors.Black; // Fallback
        var textColor = SKColors.White; // Fallback

        if (Application.Current?.TryFindResource("Color.Accent", out var aRes) == true && aRes is Color aColor)
            accentColor = new SKColor(aColor.R, aColor.G, aColor.B);

        if (Application.Current?.TryFindResource("Color.Surface", out var sRes) == true && sRes is Color sColor)
            surfaceColor = new SKColor(sColor.R, sColor.G, sColor.B);
        
        if (Application.Current?.TryFindResource("Color.Text.Primary", out var tRes) == true && tRes is Color tColor)
            textColor = new SKColor(tColor.R, tColor.G, tColor.B);

        TooltipBackgroundPaint = new SolidColorPaint(surfaceColor.WithAlpha(230));
        TooltipTextPaint = new SolidColorPaint(textColor);

        // Aktualizace spojnicového grafu (IncomeSeries)
        _incomeLineSeries.Values = [.. _lastChartData.IncomeHistory.Select(x => x.Amount)];
        _incomeLineSeries.Stroke = new SolidColorPaint(accentColor) { StrokeThickness = 3 };
        _incomeLineSeries.Fill = new SolidColorPaint(accentColor.WithAlpha(40));
        _incomeLineSeries.GeometryFill = new SolidColorPaint(accentColor);
        _incomeLineSeries.GeometryStroke = new SolidColorPaint(accentColor) { StrokeThickness = 1 };
        _incomeLineSeries.GeometrySize = 0;

        XAxes =
        [
            new Axis
            {
                Labels = [.. _lastChartData.IncomeHistory.Select(x => x.Date.ToString("dd.MM."))],
                LabelsRotation = 15,
            }
        ];

        // Aktualizace koláčového grafu (StatusSeries)
        RefreshTranslations();
    }
}
