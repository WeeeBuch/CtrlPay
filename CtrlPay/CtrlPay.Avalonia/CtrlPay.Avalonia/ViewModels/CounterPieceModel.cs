using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class CounterPieceModel : ViewModelBase
{
    [ObservableProperty]
    private decimal amount = 10.123456789000m;

    [ObservableProperty]
    private string title = "Celková suma kreditů";
}