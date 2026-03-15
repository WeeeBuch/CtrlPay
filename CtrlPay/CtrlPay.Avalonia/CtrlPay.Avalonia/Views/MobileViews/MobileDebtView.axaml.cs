using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileDebtView : UserControl
{
    public MobileDebtView()
    {
        InitializeComponent();
        DataContext = new DebtViewModel();
    }
}
