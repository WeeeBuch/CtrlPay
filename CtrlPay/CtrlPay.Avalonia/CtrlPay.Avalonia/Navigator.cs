using Avalonia.Controls;
using CtrlPay.Avalonia.Views;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia
{
    public interface INavigationService
    {
        void ShowMainWindow();
        void CloseLogin();
        void Logout(Window currentWindow);
    }

    public class NavigationService : INavigationService
    {
        private Window? _loginWindow;

        public void RegisterLogin(Window loginWindow)
        {
            _loginWindow = loginWindow;
        }

        public void ShowMainWindow()
        {
            var main = new MainWindow();
            main.DataContext = new MainViewModel(this);
            main.Show();
            AppLogger.Info($"Main window started.");
        }

        public void CloseLogin()
        {
            _loginWindow?.Close();
            AppLogger.Info($"Login window closed.");
        }

        public void Logout(Window currentWindow)
        {
            var login = new LoginWindow();
            login.Show();
            currentWindow.Close();
            AppLogger.Info("Logged out: Closed Main window and opened Login window.");
        }
    }
}
