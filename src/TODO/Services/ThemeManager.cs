using Avalonia;
using Avalonia.Styling;

namespace TODO.Services;

/// <summary>
/// Holds the current theme of the application so the converters for the design
/// can get it more easily.
/// </summary>
public static class ThemeManager
{
    public static ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Light;

    /// <summary>
    /// Refreshes the value of the <see cref="CurrentTheme"/> property after every change
    /// to the theme of the application. Gets called by App.axaml.cs and the SettingsManager.
    /// </summary>
    public static void RefreshCurrentTheme()
    {
        Application? app = Application.Current;

        if (app != null)
        {
            CurrentTheme = app.ActualThemeVariant;
        }
    }
}