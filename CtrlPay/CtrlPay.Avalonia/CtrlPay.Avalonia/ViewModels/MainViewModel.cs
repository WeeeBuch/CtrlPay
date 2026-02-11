using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Repos.Frontend;
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
            // Spuštění kontrol změn na pozadí
            AppLogger.Info($"Starting Checker...");
            _ = ChangeChecker.StartChecking();

            NavigationItems =
            [
                // Předáváme klíče, nikoliv výsledek GetString
                new NavItem("NavbarView.Dashboard", new DashboardView(), IconData.Dashboard),
                new NavItem("NavbarView.Debts", new DebtView(), IconData.Debt),
                new NavItem("NavbarView.Transactions", new TransactionView(), IconData.Cash),
                new NavItem("NavbarView.Settings", new SettingsView(), IconData.Cog),


                new NavItem("NavbarView.Customers", new CustomersListView(), IconData.Customers)
            ];

            // Výchozí stránka
            CurrentPage = NavigationItems[0].ViewModel;
            SelectedNavigationItem = NavigationItems[0];
        }

        partial void OnSelectedNavigationItemChanged(NavItem value)
        {
            if (value != null)
                CurrentPage = value.ViewModel;
        }
    }

    public partial class NavItem : ObservableObject
    {
        [ObservableProperty]
        private string name;

        public string NameKey { get; }
        public UserControl ViewModel { get; }
        public string Icon { get; }

        public NavItem(string nameKey, UserControl viewModel, string icon)
        {
            NameKey = nameKey;
            ViewModel = viewModel;
            Icon = icon;

            // Prvotní překlad
            Name = TranslationManager.GetString(NameKey);

            // Přihlášení k odběru změn jazyka
            TranslationManager.LanguageChanged.Add(UpdateName);
        }

        private void UpdateName()
        {
            if (!string.IsNullOrEmpty(NameKey))
                Name = TranslationManager.GetString(NameKey);
        }
    }

}
