 CtrlPay — Android / mobile documentation

This document describes everything related to the **Android** (and single-view / mobile) version of CtrlPay: how it starts, how navigation works, which views exist, and how to change or extend behavior.

---

## 1. How Android is detected and started

### 1.1 Entry point

- **Android project:** `CtrlPay.Avalonia.Android`
- **MainActivity:** `MainActivity.cs` — inherits `AvaloniaMainActivity<App>`, optionally customizes the app (e.g. `WithInterFont()`).
- **Target:** `net8.0-android34.0`
- **Min SDK:** 21
- **Package:** `com.CompanyName.CtrlPay.Avalonia`

References the shared UI project:

`CtrlPay.Avalonia`

which contains the same UI code as the desktop version.

### Application class

`CtrlPay.Avalonia/App.axaml.cs`

The method:

`OnFrameworkInitializationCompleted()`

branches based on **application lifetime**:

Desktop:

`IClassicDesktopStyleApplicationLifetime`

→ opens windows

- LoginWindow
- MainWindow

Mobile / Android / Browser:

`ISingleViewApplicationLifetime`

→ uses **single root view** instead of windows.

So **Android always uses** `ISingleViewApplicationLifetime` and the mobile code path inside `App.axaml.cs`.

---

## 1.2 What runs on Android (single-view path)

When

`ApplicationLifetime is ISingleViewApplicationLifetime singleView`

the following happens:

### Dark theme

`RequestedThemeVariant = ThemeVariant.Dark`

This ensures the entire UI (including dropdowns) uses the **dark theme** and remains readable on Android.

### Navigation

The app creates:

`MobileNavigationService(singleView)`

This service keeps a reference to `singleView` and switches screens by changing:

`singleView.MainView`

### Initial screen

If the app **is configured**:

```csharp
singleView.MainView =
    new MobileLoginView
    {
        DataContext = new LoginViewModel(nav)
    };
```

If the app **is not configured (first run)**:

```csharp
singleView.MainView =
    new MobileOnboardingView
    {
        DataContext = new OnboardingViewModel()
    };
```

### After onboarding

The app uses `WeakReferenceMessenger`.

When `OnboardingFinishedMessage` is sent, the root view switches to `MobileLoginView` with a new `LoginViewModel`.

Theme, language and API base URL are then applied from `SettingsManager.Current`.

---

## 2. Navigation on Android (single-view)

### 2.1 Navigation service

Interface:

`INavigationService`

Defined in:

`Navigator.cs`

Methods:

- `ShowMainWindow()` — opens the main application
- `CloseLogin()` — closes login (no-op on mobile)
- `Logout(Window?)` — returns the user back to login

### Android implementation

`MobileNavigationService`

Contains:

`ISingleViewApplicationLifetime _lifetime`

ShowMainWindow:

```csharp
_lifetime.MainView =
    new MobileMainView
    {
        DataContext = new MainViewModel(this, useMobileViews: true)
    };
```

Logout:

```csharp
_lifetime.MainView =
    new MobileLoginView
    {
        DataContext = new LoginViewModel(this)
    };
```

Logout always switches the **root view back to login**.

---

## 2.2 Main screen and tabs

Container:

`MobileMainView`

Structure:

Top:

`CurrentPage` scrollable content.

Bottom:

Navigation bar (icons) + Logout button.

The `MainViewModel` is created with:

`useMobileViews: true`

which generates **mobile navigation items**.

### Tab selection

The properties

- `SelectedNavigationItem`
- `CurrentPage`

are bound to the same `NavItem`.

Changing the selected item updates the visible view.

---

## 3. Mobile views (screens)

All mobile views are located in:

`CtrlPay.Avalonia/Views/MobileViews/`

Desktop views are located in:

`Views/DesktopViews/`

ViewModels are shared.

### 3.1 Mobile view list

| View | Purpose |
|-----|-----|
| MobileLoginView | Login form |
| MobileOnboardingView | First run setup |
| MobileAPIConnectView | API configuration |
| MobileMainView | Main mobile shell |
| MobileDashboardView | Customer dashboard |
| MobileDebtView | Customer debts |
| MobileTransactionView | Customer transactions |
| MobileSettingsView | Settings |
| MobileAccountantDashboardView | Accountant dashboard |
| MobileCustomersListView | Customers |
| MobilePaymentManagementView | Payment management |
| MobileAccountantTransactionsView | Accountant transactions |

Admin does **not** have a mobile view and uses `AdminView`.

---

## 3.2 Views by role

Defined in:

`MainViewModel.GenerateNavItems()`

when `_useMobileViews == true`.

Customer:

- Dashboard
- Debts
- Transactions
- Settings

Accountant:

- Dashboard
- Customers
- Payment management
- Accountant transactions
- Settings

Admin:

- Admin panel
- Settings

Logout is a separate button in the navigation bar.

---

## 4. API settings on Android

From the login screen the user can open **API settings**.

Method:

`LoginViewModel.OpenApiSettings()`

Creates:

- `APIConnectViewModel`
- `MobileAPIConnectView`

After Finish:

```csharp
singleView.MainView =
    new MobileLoginView
    {
        DataContext = new LoginViewModel(_navigation)
    };
```


