using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CtrlPay.Avalonia;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new ViewModels.MainViewModel();
    }
}