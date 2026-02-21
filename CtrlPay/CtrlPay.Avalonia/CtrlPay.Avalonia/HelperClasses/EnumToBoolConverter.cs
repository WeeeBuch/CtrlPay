using Avalonia.Data.Converters;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.HelperClasses;

// HelperClasses/EnumToBoolConverter.cs
public class IsOverpaidConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is StatusEnum status && status == StatusEnum.Overpaid;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
