using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using CtrlPay.Avalonia.HelperClasses;
using CtrlPay.Avalonia.Settings;
using CtrlPay.Avalonia.Styles;
using CtrlPay.Avalonia.Translations;
using CtrlPay.Avalonia.ViewModels;
using CtrlPay.Repos.Frontend;
using System.Linq;
using System.Reflection;
using CtrlPay.Repos;

namespace CtrlPay.Avalonia
{
    public partial class App : Application
    {
        private bool IsConfigured = false;

        public override void Initialize()
        {
            AppLogger.Info("===============================================================");
            AppLogger.Info($"Starting app. Version: {Assembly.GetExecutingAssembly().GetName().Version}");

            IsConfigured = !SettingsManager.Init();
            ThemeManager.Apply(SettingsManager.Current.Theme);
            TranslationManager.Apply(SettingsManager.Current.Language);
            Credentials.BaseUri = SettingsManager.Current.ConnectionString;
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                AppLogger.Info($"Starting Desktop version...");
                DisableAvaloniaDataAnnotationValidation();

                #region Debug
                if (DebugMode.StartDebug)
                {
                    AppLogger.Info($"Starting Debug Window...");
                    var debugWin = new DebugWindow
                    {
                        DataContext = new DebugWindowViewModel()
                    };
                    debugWin.Show();
                    AppLogger.Info($"Debug window started.");
                }
                #endregion

                if (IsConfigured)
                {
                    AppLogger.Info($"Starting Login Window...");
                    desktop.MainWindow = new LoginWindow();
                    AppLogger.Info($"Logn window started.");
                }
                else
                {
                    AppLogger.Info($"App is not configured, starting onboarding...");
                    desktop.MainWindow = new OnboardingWindow
                    {
                        DataContext = new OnboardingViewModel()
                    };
                    AppLogger.Info($"Onbording started.");
                }

                WeakReferenceMessenger.Default.Register<OnboardingFinishedMessage>(this, (r, m) =>
                {
                    var oldWindow = desktop.MainWindow;

                    var loginWindow = new LoginWindow();
                    desktop.MainWindow = loginWindow;
                    loginWindow.Show();

                    oldWindow?.Close();
                });

                desktop.ShutdownRequested += (s, e) =>
                {
                    SettingsManager.Save(SettingsManager.Current);
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                AppLogger.Info($"Starting Mobile or Web version...");

                if (IsConfigured)
                {
                    singleView.MainView = new MainView { DataContext = new MainViewModel() };
                }
                else
                {
                    singleView.MainView = new OnboardingView { DataContext = new OnboardingViewModel() };
                }

                WeakReferenceMessenger.Default.Register<OnboardingFinishedMessage>(this, (r, m) =>
                {
                    singleView.MainView = new MainView { DataContext = new MainViewModel() };
                });
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}