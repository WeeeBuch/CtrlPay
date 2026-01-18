using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CtrlPay.Avalonia;

public partial class DebtView : UserControl
{
    public DebtView()
    {
        InitializeComponent();
        DataContext = new ViewModels.DebtViewModel();
    }
}