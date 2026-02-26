using Avalonia.Data.Converters;
using Avalonia.Media;
using CtrlPay.Repos.Frontend;
using System;
using System.Globalization;

namespace CtrlPay.Avalonia.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StatusEnum status)
        {
            return status switch
            {
                StatusEnum.Completed => Brushes.LimeGreen,
                StatusEnum.Confirmed => Brushes.DarkGreen,
                StatusEnum.Pending => Brushes.Orange,
                StatusEnum.PartiallyPaid => Brushes.Yellow,
                StatusEnum.WaitingForPayment => Brushes.SkyBlue,
                StatusEnum.Paid => Brushes.Green,
                StatusEnum.Cancelled => Brushes.Red,
                StatusEnum.Expired => Brushes.Gray,
                StatusEnum.Overpaid => Brushes.Purple,
                _ => Brushes.Gray
            };
        }
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
