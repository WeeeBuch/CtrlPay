using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileDashboardView : UserControl
{
    public MobileDashboardView()
    {
        InitializeComponent();
        DataContext = new DashboardViewModel();
    }
}
