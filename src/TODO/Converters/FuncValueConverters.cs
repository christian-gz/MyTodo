using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using TODO.Services;

namespace TODO.Converters;

public static class FuncValueConverters
{
    /// <summary>
    /// Background for the menu items of the navigation bar
    /// </summary>
    public static FuncValueConverter<bool, Brush?> CurrentViewToBrush { get; } =
        new FuncValueConverter<bool, Brush?>(b =>
        {
            ThemeVariant themeVariant = ThemeManager.CurrentTheme;

            if (b)
            {
                return new SolidColorBrush(themeVariant == ThemeVariant.Light ? Colors.White : Color.Parse("#16161a"));
            }

            return new SolidColorBrush(themeVariant == ThemeVariant.Light ? Color.Parse("#f0f0f0") : Color.Parse("#202020"));
        });

    /// <summary>
    /// Datetime format for the calendar date picker
    /// </summary>
    public static FuncValueConverter<DateTime?, string?> DateTimeFormatter { get; } =
        new FuncValueConverter<DateTime?, string?>(d => d?.ToString("d MMM yyyy"));

    /// <summary>
    /// Background for the TodoItem depending on the checked status
    /// </summary>
    public static FuncValueConverter<bool, Brush?> IsCheckedToBackgroundBrushConverter { get; } =
        new FuncValueConverter<bool, Brush?>(b =>
        {
            ThemeVariant themeVariant = ThemeManager.CurrentTheme;

            if (b)
            {
                return new SolidColorBrush(themeVariant == ThemeVariant.Light ? Color.Parse("#e6e8ea") : Color.Parse("#1e1e22"));
            }

            return new SolidColorBrush(themeVariant == ThemeVariant.Light ? Colors.White : Color.Parse("#16161a"));
        });

    /// <summary>
    /// Border for the TodoItem depending on the checked status
    /// </summary>
    public static FuncValueConverter<bool, Brush?> IsCheckedToBorderBrushConverter { get; } =
        new FuncValueConverter<bool, Brush?>(b =>
        {
            ThemeVariant themeVariant = ThemeManager.CurrentTheme;

            if (b)
            {
                return new SolidColorBrush(themeVariant == ThemeVariant.Light ? Colors.White : Color.Parse("#16161a"));
            }

            return new SolidColorBrush(Colors.DimGray);
        });

    /// <summary>
    /// Text decoration for the TodoItem depending on the edit status
    /// </summary>
    public static FuncValueConverter<bool, TextDecorationCollection?> IsCheckedToTextDecoration { get; } =
        new FuncValueConverter<bool, TextDecorationCollection?>(b =>
            TextDecorationCollection.Parse(b ? "Strikethrough" : "")
        );
}