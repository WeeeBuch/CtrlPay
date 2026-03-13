using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using CtrlPay.Avalonia;
using CtrlPay.Avalonia.Views.MobileViews;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;

namespace CtrlPay.Avalonia.ViewModels;

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

    public ObservableCollection<NavItem> NavigationItems { get; private set; }

    private readonly INavigationService _navigation;
    private readonly bool _useMobileViews;

    public MainViewModel(INavigationService navigation, bool useMobileViews = false)
    {
        _navigation = navigation;
        _useMobileViews = useMobileViews;
        // Spuštění kontrol změn na pozadí
        AppLogger.Info($"Starting Checker...");
        _ = ChangeChecker.StartChecking();

        GenerateNavItems();

        CurrentPage = NavigationItems[0].ViewModel;
        SelectedNavigationItem = NavigationItems[0];

        // Nasloucháme zprávám z Dashboardu pro navigaci s filtrem
        WeakReferenceMessenger.Default.Register<NavigationFilterMessage>(this, (r, m) => HandleNavigationFilter(m.Filter));
    }

    [RelayCommand]
    private void Logout(Window? currentWindow)
    {
        AuthRepo.Logout();
        _navigation.Logout(currentWindow!);
    }

    private void HandleNavigationFilter(StatusEnum filter)
    {
        // Najdeme položku v menu, která odpovídá správě plateb
        var target = NavigationItems.FirstOrDefault(i => i.ViewModel is PaymentManagementView);
        if (target != null)
        {
            SelectedNavigationItem = target;
            // Nastavíme filtr v cílovém ViewModelu
            if (target.ViewModel.DataContext is PaymentManagementViewModel vm)
            {
                vm.SelectedStatusItem = vm.Statuses.FirstOrDefault(s => s.Value == filter);
            }
        }
    }

    private void GenerateNavItems()
    {
        Role role;
#if DEBUG
        if (DebugMode.OverrideRole)
        {
            role = DebugMode.DebugRole;
        }
        else
#endif
        {
            role = Credentials.Role;
        }

        NavigationItems = [];

        if (_useMobileViews)
        {
            if (role == Role.Customer)
            {
                NavigationItems.Add(new NavItem("NavbarView.Dashboard", new MobileDashboardView(), IconData.Dashboard));
                NavigationItems.Add(new NavItem("NavbarView.Debts", new MobileDebtView(), IconData.Debt));
                NavigationItems.Add(new NavItem("NavbarView.Transactions", new MobileTransactionView(), IconData.Cash));
            }
            else if (role == Role.Accountant)
            {
                NavigationItems.Add(new NavItem("NavbarView.Dashboard", new AccountantDashboardView(), IconData.Dashboard));
                NavigationItems.Add(new NavItem("NavbarView.Customers", new CustomersListView(), IconData.Customers));
                NavigationItems.Add(new NavItem("NavbarView.PaymentManagement", new PaymentManagementView(), IconData.Cash));
                NavigationItems.Add(new NavItem("NavbarView.AccountantTransactions", new AccountantTransactionsView(), IconData.Cash));
            }
            else if (role == Role.Admin)
            {
                NavigationItems.Add(new NavItem("NavbarView.AdminPanel", new AdminView(), IconData.Admin));
            }
            NavigationItems.Add(new NavItem("NavbarView.Settings", new MobileSettingsView(), IconData.Cog));
        }
        else
        {
            if (role == Role.Customer)
            {
                NavigationItems.Add(new NavItem("NavbarView.Dashboard", new DashboardView(), IconData.Dashboard));
                NavigationItems.Add(new NavItem("NavbarView.Debts", new DebtView(), IconData.Debt));
                NavigationItems.Add(new NavItem("NavbarView.Transactions", new TransactionView(), IconData.Cash));
            }

            if (role == Role.Accountant)
            {
                NavigationItems.Add(new NavItem("NavbarView.Dashboard", new AccountantDashboardView(), IconData.Dashboard));
                NavigationItems.Add(new NavItem("NavbarView.Customers", new CustomersListView(), IconData.Customers));
                NavigationItems.Add(new NavItem("NavbarView.PaymentManagement", new PaymentManagementView(), IconData.Cash));
                NavigationItems.Add(new NavItem("NavbarView.AccountantTransactions", new AccountantTransactionsView(), IconData.Cash));
            }

            if (role == Role.Admin)
            {
                NavigationItems.Add(new NavItem("NavbarView.AdminPanel", new AdminView(), IconData.Admin));
            }

            if (role == Role.Employee)
            {

            }

            NavigationItems.Add(new NavItem("NavbarView.Settings", new SettingsView(), IconData.Cog));
        }
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
