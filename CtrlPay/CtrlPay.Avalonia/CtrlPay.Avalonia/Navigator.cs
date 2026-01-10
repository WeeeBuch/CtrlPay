using Avalonia.Controls;
using CtrlPay.Avalonia.Views;
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
            main.Show();
        }

        public void CloseLogin()
        {
            _loginWindow?.Close();
        }
    }
}
