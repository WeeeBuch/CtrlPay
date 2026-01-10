using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace CtrlPay.Avalonia.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private object _currentPage;

        [ObservableProperty]
        private NavItem _selectedNavigationItem;

        [ObservableProperty]
        private string dashboardIcon = IconData.Dashboard;

        public ObservableCollection<NavItem> NavigationItems { get; }

        public MainViewModel()
        {
            NavigationItems =
            [
                new NavItem("Nastavení", new SettingsThemeView(), IconData.CogOutline)
            ];

            // Výchozí stránka
            CurrentPage = NavigationItems[0].ViewModel;
            SelectedNavigationItem = NavigationItems[0];
        }

        partial void OnSelectedNavigationItemChanged(NavItem? value)
        {
            if (value != null)
                CurrentPage = value.ViewModel;
        }
    }

    public record NavItem(string Name, UserControl ViewModel, string Icon);

}
