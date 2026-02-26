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
    // Grafy
    [ObservableProperty] private ISeries[] _incomeSeries;
    [ObservableProperty] private Axis[] _xAxes;
    [ObservableProperty] private ISeries[] _statusSeries;

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
    }

    private void LoadData()
    {
        // Načteme souhrnná data z ToDoRepo (zatím Mock data)
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
        var chartData = ToDoRepo.GetAccountantChartData();

        // Získáme akcentní barvu z aplikace
        var accentColor = SKColors.CornflowerBlue; // Fallback

        // 1. Graf příjmů (Trend) s akcentní barvou a překladem
        IncomeSeries =
        [
            new LineSeries<decimal>
            {
                Values = [.. chartData.IncomeHistory.Select(x => x.Amount)],
                Name = TranslationManager.GetString("Accountant.Dashboard.IncomeHistory"),
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
                Labels = [.. chartData.IncomeHistory.Select(x => x.Date.ToString("dd.MM."))],
                LabelsRotation = 15,
            }
        ];

        // 2. Graf stavů (Koláč -> Donut) s logickými barvami a překlady
        StatusSeries = [.. chartData.StatusBreakdown.Select(x => new PieSeries<int>
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
}
