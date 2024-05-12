using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TODO.Converters;

// Deadline and checked status of todoItem to border color
public class TodoStateToBrushConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return BindingOperations.DoNothing;
        }

        DateTime? deadline = values[0] as DateTime? ?? null;
        bool isChecked = values[1] as bool? ?? false;

        if (deadline == null || isChecked)
        {
            return new SolidColorBrush(Colors.Black);
        }

        DateTime now = DateTime.Now.Date;
        TimeSpan delta = ((DateTime)deadline).Date.Subtract(now);

        if (delta.Days < 1)
        {
            return new SolidColorBrush(Color.Parse("#EF5350"));
        }
        if (delta.Days <= 3)
        {
            return new SolidColorBrush(Color.Parse("#FFC107"));
        }
        if (delta.Days > 3)
        {
            return new SolidColorBrush(Color.Parse("#8BC34A"));
        }

        return new SolidColorBrush(Colors.Black);
    }
}