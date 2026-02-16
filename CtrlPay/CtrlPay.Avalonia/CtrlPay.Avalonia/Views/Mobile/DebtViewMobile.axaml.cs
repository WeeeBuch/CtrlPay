using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.Mobile;

public partial class DebtViewMobile : UserControl
{
    public DebtViewMobile()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new DebtViewModel();

    }
}