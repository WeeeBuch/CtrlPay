using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class TransactionListPiece : UserControl
{
    public TransactionListPiece()
    {
        InitializeComponent();
        DataContext = new TransactionListPieceModel();
    }
}