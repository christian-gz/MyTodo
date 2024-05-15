using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using TODO.Configuration;
using TODO.ViewModels;
using TODO.Views;

namespace TODO;

public partial class App : Application
{
    private SettingsManager _settingsManager = new SettingsManager();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            LoadTheme();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(_settingsManager),
            };
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
    }
}