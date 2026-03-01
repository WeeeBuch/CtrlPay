using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class AccountantDashboardView : UserControl
{
    public AccountantDashboardView()
    {
        InitializeComponent();
        // Nastavení DataContextu, aby View vědělo, odkud brát data
        DataContext = new AccountantDashboardViewModel();
    }
}
