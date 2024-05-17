using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using TODO.Configuration;
using TODO.Events;
using TODO.Services;
using TODO.ViewModels;
using TODO.Views;

namespace TODO;

public partial class App : Application
{
    private SettingsManager _settingsManager;
    private SoundService _soundService;
    private TodoListService _todoListService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // save all ApplicationEvents that are raises during initialisation
            List<ApplicationEventArgs> eventArgsList = new();
            EventHandler<ApplicationEventArgs> collectApplicationEvents = (o, e) => eventArgsList.Add(e);
            EventManager.ApplicationEvent += collectApplicationEvents;

            _soundService = new SoundService();
            _settingsManager = new SettingsManager(_soundService);
            _todoListService = new TodoListService(_settingsManager);

            LoadTheme();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(_settingsManager, _todoListService),
            };

            // re-raise all ApplicationEvents now that the application is ready to handle them
            EventManager.ApplicationEvent -= collectApplicationEvents;
            foreach (var eventArg in eventArgsList)
            {
                EventManager.RaiseEvent(eventArg);
            }
            eventArgsList.Clear();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void LoadTheme()
    {
        var isDarkMode = _settingsManager.GetDarkModeEnabled();
        if (isDarkMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            RequestedThemeVariant = ThemeVariant.Light;
        }
        ThemeManager.RefreshCurrentTheme();
    }
}