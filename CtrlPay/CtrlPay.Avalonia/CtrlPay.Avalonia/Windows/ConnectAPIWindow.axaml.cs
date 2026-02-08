using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CtrlPay.Avalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia;

public partial class ConnectAPIWindow : Window
{
    public ConnectAPIWindow()
    {
        InitializeComponent();

        // Předpokládáme, že ViewModel je nastaven v DataContextu
        DataContextChanged += (s, e) =>
        {
            if (DataContext is APIConnectViewModel vm)
            {
                // Tímto říkáme ViewModelu: "Až zavoláš CloseAction, já zavřu okno"
                vm.CloseAction = () => Close();
            }
        };
    }
}