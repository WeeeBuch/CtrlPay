using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CtrlPay.Avalonia;

#if DEBUG
public partial class DebugWindow : Window
{
    public DebugWindow()
    {
        InitializeComponent();
    }
}
#else
public partial class DebugWindow : Window
{
    public DebugWindow() { }
}
#endif