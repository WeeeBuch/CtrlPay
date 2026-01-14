using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class CounterPiece : UserControl
{
    public CounterPiece()
    {
        InitializeComponent();
        DataContext = new CounterPieceModel();
    }
}