using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.ObjectModel;
using CtrlPay.Avalonia.Views.Mobile;
using System.Xml.Linq;

namespace CtrlPay.Avalonia.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        // D�LE�IT�: CurrentPage m� b�t Control (view), ne object
        [ObservableProperty]
        private Control? currentPage;

        // D�LE�IT�: je nullable, proto�e p�i loginu nem� vybranou polo�ku
        [ObservableProperty]
        private NavItem? selectedNavigationItem;

        [ObservableProperty]
        private bool isPaneOpen = true;

        public double PaneWidth => IsPaneOpen ? 200 : 65;

        [RelayCommand]
        private void TogglePane()
        {
            IsPaneOpen = !IsPaneOpen;
            OnPropertyChanged(nameof(PaneWidth));
        }

        [ObservableProperty]
        private string menuIcon = IconData.Menu;

        private readonly CtrlPay.Avalonia.INavigationService _navigation;

        public ObservableCollection<NavItem> NavigationItems { get; }

        public MainViewModel()
        {
            _navigation = new CtrlPay.Avalonia.NavigationService(this);

            if (OperatingSystem.IsAndroid())
            {
                NavigationItems = new()
                {
                    new NavItem("NavbarView.Dashboard", new DashboardViewMobile(), IconData.Dashboard),
                    new NavItem("NavbarView.Debts", new DebtViewMobile(), IconData.Debt),
                    new NavItem("NavbarView.Settings", new SettingsViewMobile(), IconData.Cog),
                    new NavItem("NavbarView.Transactions", new TransactionViewMobile(), IconData.Cash)

                };

                CurrentPage = new LoginViewMobile
                {
                    DataContext = new LoginViewModel(_navigation)
                };

                SelectedNavigationItem = null;
            }
            else
            {
                NavigationItems = new()
                {
                    new NavItem("NavbarView.Dashboard", new DashboardView(), IconData.Dashboard),
                    new NavItem("NavbarView.Debts", new DebtView(), IconData.Debt),
                    new NavItem("NavbarView.Settings", new SettingsView(), IconData.Cog),
                    new NavItem("NavbarView.Transaction", new TransactionViewMobile(), IconData.Customers)

                };

                CurrentPage = new LoginView
                {
                    DataContext = new LoginViewModel(_navigation)
                };

                SelectedNavigationItem = null;
            }
        }

        // D�LE�IT�: nullable parametr
        partial void OnSelectedNavigationItemChanged(NavItem? value)
        {
            if (value != null)
                CurrentPage = value.View; // viz n�e v NavItem
        }



    }

    public partial class NavItem : ObservableObject
    {
        [ObservableProperty]
        private string name = "";

        public string NameKey { get; }
        public UserControl View { get; }   // p�ejmenov�no z ViewModel (proto�e je to View)
        public string Icon { get; }

        public NavItem(string nameKey, UserControl view, string icon)
        {
            NameKey = nameKey;
            View = view;
            Icon = icon;

            UpdateName();
            TranslationManager.LanguageChanged.Add(UpdateName);
        }

        private void UpdateName()
        {
            if (!string.IsNullOrEmpty(NameKey))
                Name = TranslationManager.GetString(NameKey);
        }



    }
}
