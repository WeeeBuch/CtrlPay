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

        // 1. Graf příjmů (Trend)
        IncomeSeries =
        [
            new LineSeries<decimal>
            {
                Values = [.. chartData.IncomeHistory.Select(x => x.Amount)],
                Name = "Příjem (XMR)",
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

        // 2. Graf stavů (Koláč)
        StatusSeries = [.. chartData.StatusBreakdown.Select(x => new PieSeries<int>
        {
            Values = [x.Count],
            Name = x.Status
        })];
    }
}
