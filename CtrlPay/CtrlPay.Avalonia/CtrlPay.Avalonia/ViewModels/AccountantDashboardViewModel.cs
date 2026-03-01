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

        LoadData();

        UpdateHandler.UpdatedData.Add(LoadData);
        
        // Přihlášení ke změně jazyka
        TranslationManager.LanguageChanged.Add(RefreshTranslations);
    }

    private void RefreshTranslations()
    {
        if (_lastChartData == null) return;

        // Aktualizujeme popisky v koláčovém grafu
        StatusSeries = [.. _lastChartData.StatusBreakdown.Select(x => new PieSeries<int>
        {
            Values = [x.Count],
            Name = TranslationManager.GetString($"Transaction.Status.{x.Status}"),
            Fill = x.Status switch
            {
                "Paid" => new SolidColorPaint(SKColors.MediumSeaGreen),
                "Overpaid" => new SolidColorPaint(SKColors.CornflowerBlue),
                "PartiallyPaid" => new SolidColorPaint(SKColors.Orange),
                "Expired" => new SolidColorPaint(SKColors.Crimson),
                _ => new SolidColorPaint(SKColors.Gray)
            },
            InnerRadius = 60
        })];
    }

    private void LoadData()
    {
        // Načteme souhrnná data z ToDoRepo (zatím Mock data)
        // TODO: Karele tu pak nezapomeň přepsat
        var summary = ToDoRepo.GetAccountantDashboardSummary();

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
        var data = ToDoRepo.GetAccountantChartData();

        if (data == _lastChartData) return;

        _lastChartData = data;

        // Získáme akcentní barvu z aplikace
        var accentColor = SKColors.CornflowerBlue; // Fallback

        // Nastavení barev pro tooltipy (Tmavý režim)
        var surfaceColor = SKColors.Black; // Fallback
        var textColor = SKColors.White; // Fallback

        if (Application.Current?.TryFindResource("Color.Surface", out var sRes) == true && sRes is Color sColor)
            surfaceColor = new SKColor(sColor.R, sColor.G, sColor.B);
        
        if (Application.Current?.TryFindResource("Color.Text.Primary", out var tRes) == true && tRes is Color tColor)
            textColor = new SKColor(tColor.R, tColor.G, tColor.B);

        TooltipBackgroundPaint = new SolidColorPaint(surfaceColor.WithAlpha(230)); // Lehce průhledná tmavá
        TooltipTextPaint = new SolidColorPaint(textColor);

        // 1. Graf příjmů (Trend)
        IncomeSeries =
        [
            new LineSeries<decimal>
            {
                Values = [.. _lastChartData.IncomeHistory.Select(x => x.Amount)],
                Fill = new SolidColorPaint(accentColor.WithAlpha(40)),
                Stroke = new SolidColorPaint(accentColor) { StrokeThickness = 3 },
                GeometrySize = 0,
                LineSmoothness = 1
            }
        ];

        XAxes =
        [
            new Axis
            {
                Labels = [.. _lastChartData.IncomeHistory.Select(x => x.Date.ToString("dd.MM."))],
                LabelsRotation = 15,
            }
        ];

        // 2. Graf stavů (Koláč -> Donut) s logickými barvami a překlady
        // Odebraná duplicita kódu
        RefreshTranslations();
    }
}
