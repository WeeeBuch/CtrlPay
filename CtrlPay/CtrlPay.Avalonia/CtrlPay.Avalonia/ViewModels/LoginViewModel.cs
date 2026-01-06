using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CtrlPay.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string title = "Welcome to Avalonia!";

    [ObservableProperty]
    private string username;

    [RelayCommand]
    private void Login()
    {
        Title = $"Login button clicked! Logged as {Username}";
    }
}
