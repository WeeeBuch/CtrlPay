using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

    /// <summary>
    /// Mobile / single-view implementation of navigation.
    /// Uses ISingleViewApplicationLifetime and swaps the root view instead of opening windows.
    /// </summary>
    public class MobileNavigationService : INavigationService
    {
        private readonly ISingleViewApplicationLifetime _lifetime;

        public MobileNavigationService(ISingleViewApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        public void ShowMainWindow()
        {
            var mainView = new MainView
            {
                DataContext = new MainViewModel(this)
            };

            _lifetime.MainView = mainView;
            AppLogger.Info("Main view started (mobile single-view).");
        }

        public void CloseLogin()
        {
            // Not needed for single-view; ShowMainWindow replaces the root view.
        }

        public void Logout(Window currentWindow)
        {
            var loginView = new LoginView
            {
                DataContext = new LoginViewModel(this)
            };

            _lifetime.MainView = loginView;
            AppLogger.Info("Logged out: switched to LoginView (mobile single-view).");
        }
    }
}
