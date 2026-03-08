using Avalonia.Data.Converters;
using Avalonia.Media;
using CtrlPay.Entities;
using System;
using System.Globalization;

namespace CtrlPay.Avalonia.Converters;

public class TransactionTypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TransactionTypeEnum type)
        {
            return type == TransactionTypeEnum.Incoming ? Brushes.LimeGreen : Brushes.OrangeRed;
        }
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
