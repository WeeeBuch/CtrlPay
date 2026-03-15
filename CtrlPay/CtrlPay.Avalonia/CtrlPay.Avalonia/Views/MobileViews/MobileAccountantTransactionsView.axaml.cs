using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileAccountantTransactionsView : UserControl
{
    public MobileAccountantTransactionsView()
    {
        InitializeComponent();
        DataContext = new AccountantTransactionsViewModel();
    }
}
