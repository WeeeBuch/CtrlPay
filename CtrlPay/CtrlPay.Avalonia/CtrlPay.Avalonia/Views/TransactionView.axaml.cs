using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class TransactionView : UserControl
{
    public TransactionView()
    {
        InitializeComponent();
        DataContext = new TransactionViewModel();
    }
}