using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileTransactionView : UserControl
{
    public MobileTransactionView()
    {
        InitializeComponent();
        DataContext = new TransactionViewModel();
    }
}
