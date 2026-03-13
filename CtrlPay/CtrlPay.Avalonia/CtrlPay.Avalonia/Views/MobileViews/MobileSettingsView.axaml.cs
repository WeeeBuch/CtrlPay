using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileSettingsView : UserControl
{
    public MobileSettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }
}
