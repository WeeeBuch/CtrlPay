using Avalonia.Controls;

namespace CtrlPay.Avalonia;

public partial class AccountantDashboardView : UserControl
{
    public AccountantDashboardView()
    {
        InitializeComponent();
        // Poznámka: DataContext se většinou nastavuje v MainViewModelu při navigaci,
        // ale pro jistotu (nebo design-time) ho můžeme inicializovat i zde.
    }
}
