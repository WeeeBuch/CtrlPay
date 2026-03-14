using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobilePaymentManagementView : UserControl
{
    public MobilePaymentManagementView()
    {
        InitializeComponent();
        DataContext = new PaymentManagementViewModel();
    }
}
