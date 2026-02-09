using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Avalonia.Views;

namespace CtrlPay.Avalonia;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        /*
        var navigation = new NavigationService();
        navigation.RegisterLogin(this);

        DataContext = new LoginViewModel(navigation);*/
    }
}