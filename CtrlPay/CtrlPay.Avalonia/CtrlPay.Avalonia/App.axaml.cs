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
using CtrlPay.Avalonia.Views;
using CtrlPay.Repos.Frontend;
using System;
using System.Linq;

namespace CtrlPay.Avalonia
{
    public partial class App : Application
    {
        private bool IsConfigured = false;

        public override void Initialize()
        {
            IsConfigured = !SettingsManager.Init();
            ThemeManager.Apply(SettingsManager.Current.Theme);
            TranslationManager.Apply(SettingsManager.Current.Language);
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                #region Debug
                if (DebugMode.StartDebug)
                {
                    var debugWin = new DebugWindow
                    {
                        DataContext = new DebugWindowViewModel()
                    };
                    debugWin.Show();
                }
                #endregion

                if (IsConfigured)
                {
                    desktop.MainWindow = new LoginWindow();
                }
                else
                {
                    desktop.MainWindow = new OnboardingWindow
                    {
                        DataContext = new OnboardingViewModel()
                    };
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