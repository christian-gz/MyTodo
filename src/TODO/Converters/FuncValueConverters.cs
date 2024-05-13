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
            return new SolidColorBrush(Color.Parse("#f0f0f0"));
        });

    // Datetime format for the calendar date picker
    public static FuncValueConverter<DateTime?, string?> DateTimeFormatter { get; } =
        new FuncValueConverter<DateTime?, string?>(d => d?.ToString("d MMM yyyy"));

    // Background for the TodoItem depending on the checked status
    public static FuncValueConverter<bool, Brush?> IsCheckedToBackgroundBrushConverter { get; } =
        new FuncValueConverter<bool, Brush?>(b => new SolidColorBrush(b ? Color.Parse("#e6e8ea") : Colors.White));

    // Border for the TodoItem depending on the checked status
    public static FuncValueConverter<bool, Brush?> IsCheckedToBorderBrushConverter { get; } =
        new FuncValueConverter<bool, Brush?>(b => new SolidColorBrush(b ? Colors.White: Colors.DimGray));

    // Text decoration for the TodoItem depending on the edit status
    public static FuncValueConverter<bool, TextDecorationCollection?> IsCheckedToTextDecoration { get; } =
        new FuncValueConverter<bool, TextDecorationCollection?>(b =>
            TextDecorationCollection.Parse(b ? "Strikethrough" : "")
        );
}