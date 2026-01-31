using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.Mobile;

public partial class DashboardViewMobile : UserControl
{
    public DashboardViewMobile()
    {
        AvaloniaXamlLoader.Load(this);
    }
}