using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class AccountantTransactionsView : UserControl
{
    public AccountantTransactionsView()
    {
        InitializeComponent();
        DataContext = new AccountantTransactionsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
