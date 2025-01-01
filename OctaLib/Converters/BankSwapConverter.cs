using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OctaLibAvalonia.Converters;

public class BankSwapConverter : IMultiValueConverter
{

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 0 || values[0] is Avalonia.UnsetValueType) 
        {
            return null;
        }

        return new Tuple<string, string>((string)values[0], (string)values[1]);
    }
}

