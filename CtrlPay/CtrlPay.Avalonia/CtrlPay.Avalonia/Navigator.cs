using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Avalonia.Views;
using CtrlPay.Avalonia.Views.Mobile;
using CtrlPay.Entities;
using CtrlPay.Repos;
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
        void NavigateToLogin();
        void NavigateToMain();
        void NavigateToOnboarding();
    }

    public class NavigationService : INavigationService
    {
        private readonly ISingleViewApplicationLifetime _lifetime;

        public NavigationService(ISingleViewApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        public void NavigateToLogin()
        {
            _lifetime.MainView = new LoginView
            {
                DataContext = new LoginViewModel(this)
            };
        }

        public void NavigateToMain()
        {
            _lifetime.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        public void NavigateToOnboarding()
        {
            _lifetime.MainView = new OnboardingView
            {
                DataContext = new OnboardingViewModel()
            };
        }
    }
}