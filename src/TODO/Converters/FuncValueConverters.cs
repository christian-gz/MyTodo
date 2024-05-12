using System;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TODO.Converters;

public static class FuncValueConverters
{
    // Background for the menu items of the navigation bar
    public static FuncValueConverter<bool, Brush?> CurrentViewToBrush { get; } =
        new FuncValueConverter<bool, Brush?>(b =>
        {
            if (b)
            {
                return new SolidColorBrush(Colors.White);
            }
            return new SolidColorBrush(Colors.Gainsboro);
        });

    // Border thickness for the todoItem depending on the edit status
    public static FuncValueConverter<bool, Thickness> EditStatusConverter { get; } =
        new FuncValueConverter<bool, Thickness>(b => new Thickness(b ? 2 : 1));

    // Datetime format for the calendar date picker
    public static FuncValueConverter<DateTime?, string?> DateTimeFormatter { get; } =
        new FuncValueConverter<DateTime?, string?>(d => d?.ToString("d MMM yyyy"));

    // Background for the todoItem depending on the checked status
    public static FuncValueConverter<bool, Brush?> IsCheckedToBrushConverter { get; } =
        new FuncValueConverter<bool, Brush?>(b => new SolidColorBrush(b ? Color.Parse("#EEEEEE") : Colors.White));
}