using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Avalonia.Views.Mobile;

namespace CtrlPay.Avalonia.Views.Mobile;

public partial class TransactionViewMobile : UserControl
{
    public TransactionViewMobile()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new TransactionViewModel();

    }
}