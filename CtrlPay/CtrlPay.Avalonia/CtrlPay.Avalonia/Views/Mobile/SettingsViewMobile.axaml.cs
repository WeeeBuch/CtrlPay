using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.Mobile;

public partial class SettingsViewMobile : UserControl
{
    public SettingsViewMobile()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new SettingsViewModel();
    }
}