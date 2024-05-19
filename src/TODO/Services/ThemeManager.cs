using Avalonia;
using Avalonia.Styling;
using TODO.Configuration;

namespace TODO.Services;

/// <summary>
/// Holds the current theme of the application so the converters for the design
/// can get it more easily.
/// </summary>
public class ThemeManager
{
    private readonly SettingsManager _settingsManager;
    public static ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Light;

    public ThemeManager(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        bool darkModeEnabled = _settingsManager.GetDarkModeEnabled();
        SetApplicationTheme(darkModeEnabled ? ThemeVariant.Dark : ThemeVariant.Light);

        _settingsManager.SettingsChange += HandleSettingsChange;
    }

    private void HandleSettingsChange(object? o, SettingsChangedEventArgs e)
    {
        if (e.Setting == "DarkModeEnabled")
        {
            bool value = bool.Parse(e.Value);
            SetApplicationTheme(value ? ThemeVariant.Dark : ThemeVariant.Light);
        }
    }

    /// <summary>
    /// Sets the theme of the application after every switch of the setting.
    /// </summary>
    public void SetApplicationTheme(ThemeVariant themeVariant)
    {
        Application? app = Application.Current;

        if (app != null)
        {
            app.RequestedThemeVariant = themeVariant;
        }

        CurrentTheme = themeVariant;
    }
}