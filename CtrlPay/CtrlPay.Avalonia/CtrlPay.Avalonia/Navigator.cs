using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Avalonia.Views;
using CtrlPay.Avalonia.Views.Mobile;
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
        void NavigateToMain();
        void NavigateToLogin();
    }

    public sealed class NavigationService : INavigationService
    {
        private readonly MainViewModel _main;

        public NavigationService(MainViewModel main)
        {
            _main = main;
        }

        public void NavigateToMain()
        {
            var first = _main.NavigationItems[0];
            _main.SelectedNavigationItem = first;
            _main.CurrentPage = first.View; // tady dáváš UserControl (View)
        }

        public void NavigateToLogin()
        {
            _main.SelectedNavigationItem = null;

            if (OperatingSystem.IsAndroid())
            {
                _main.CurrentPage = new LoginViewMobile
                {
                    DataContext = new LoginViewModel(this)
                };
            }
            else
            {
                _main.CurrentPage = new LoginView
                {
                    DataContext = new LoginViewModel(this)
                };
            }
        }
    }
}