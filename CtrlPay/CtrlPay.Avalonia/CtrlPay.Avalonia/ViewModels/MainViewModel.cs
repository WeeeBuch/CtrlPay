using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
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
        private bool _isPaneOpen = true;

        public double PaneWidth => IsPaneOpen ? 200 : 65;

        // A uprav TogglePane, aby o tom dal vědět
        [RelayCommand]
        private void TogglePane()
        {
            IsPaneOpen = !IsPaneOpen;
            OnPropertyChanged(nameof(PaneWidth));
        }

        [ObservableProperty]
        private string menuIcon = IconData.Menu;

        public ObservableCollection<NavItem> NavigationItems { get; }

        public MainViewModel()
        {
            NavigationItems =
            [
                new NavItem(TranslationManager.GetString("NavbarView.Dashboard"), new DashboardView(), IconData.Dashboard),
                new NavItem(TranslationManager.GetString("NavbarView.Settings"), new SettingsView(), IconData.Cog)
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
