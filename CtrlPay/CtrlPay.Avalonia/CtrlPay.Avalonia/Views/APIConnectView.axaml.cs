using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia;

public partial class APIConnectView : UserControl
{
    public APIConnectView()
    {
        InitializeComponent();
        DataContext = new APIConnectViewModel();
    }
}