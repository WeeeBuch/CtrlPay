using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class AdminView : UserControl
{
    public AdminView()
    {
        InitializeComponent();
        DataContext = new AdminViewModel();
    }
}
