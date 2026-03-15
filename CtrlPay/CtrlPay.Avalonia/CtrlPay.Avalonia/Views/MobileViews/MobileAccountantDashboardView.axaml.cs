using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileAccountantDashboardView : UserControl
{
    public MobileAccountantDashboardView()
    {
        InitializeComponent();
        DataContext = new AccountantDashboardViewModel();
    }
}
