using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class PaymentManagementView : UserControl
{
    public PaymentManagementView()
    {
        InitializeComponent();
        DataContext = new PaymentManagementViewModel();
    }

    private void ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}