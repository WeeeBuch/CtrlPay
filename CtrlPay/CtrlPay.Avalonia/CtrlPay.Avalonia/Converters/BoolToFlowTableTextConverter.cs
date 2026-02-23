using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace CtrlPay.Avalonia.Converters;

public class BoolToFlowTableTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFlow)
        {
            return isFlow ? "Switch to Table" : "Switch to Flow";
        }
        return "Switch View";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
