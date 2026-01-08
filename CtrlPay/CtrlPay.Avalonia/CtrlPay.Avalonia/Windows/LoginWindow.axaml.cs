using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Avalonia.Views;

namespace CtrlPay.Avalonia;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();

        var vm = new LoginViewModel();
        vm.LoginSucceeded += OnLoginSucceeded;

        DataContext = vm;
    }

    private void OnLoginSucceeded()
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();

        Close();
    }
}