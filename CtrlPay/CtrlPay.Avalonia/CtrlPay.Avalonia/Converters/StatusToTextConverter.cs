using Avalonia.Data.Converters;
using CtrlPay.Repos.Frontend;
using System;
using System.Globalization;

namespace CtrlPay.Avalonia.Converters;

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StatusEnum status)
        {
            // Tady by ideálně měly být překlady z TranslationManageru
            return status switch
            {
                StatusEnum.Pending => "Čekající",
                StatusEnum.Completed => "Dokončeno",
                StatusEnum.Confirmed => "Potvrzeno",
                StatusEnum.Overpaid => "Přeplatek",
                StatusEnum.PartiallyPaid => "Částečně zaplaceno",
                StatusEnum.Expired => "Po splatnosti",
                StatusEnum.Cancelled => "Zrušeno",
                _ => status.ToString()
            };
        }
        return "Všechny stavy";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
