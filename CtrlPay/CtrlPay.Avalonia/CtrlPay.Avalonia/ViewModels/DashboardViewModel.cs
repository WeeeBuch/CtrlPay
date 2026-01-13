using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    // Instance pro první čítač
    public CounterPieceModel TotalCredits { get; } = new()
    {
        Title = "Celková suma kreditů",
        Amount = 20.000000000000m
    };

    // Instance pro druhý čítač
    public CounterPieceModel PendingCredits { get; } = new()
    {
        Title = "Čekající platby",
        Amount = 1.520000000000m
    };
}