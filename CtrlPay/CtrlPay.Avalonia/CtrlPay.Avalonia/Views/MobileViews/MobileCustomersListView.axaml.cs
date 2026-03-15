using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views.MobileViews;

public partial class MobileCustomersListView : UserControl
{
    public MobileCustomersListView()
    {
        InitializeComponent();
        DataContext = new CustomersListViewModel();
    }
}
