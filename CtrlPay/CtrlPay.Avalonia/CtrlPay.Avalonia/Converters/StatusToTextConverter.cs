using Avalonia.Data.Converters;
using CtrlPay.Avalonia.Translations;
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
            // Použijeme stávající klíče pro statusy transakcí
            return TranslationManager.GetString($"Transaction.Status.{status}");
        }
        
        // Pro volbu null (všechny stavy)
        return TranslationManager.GetString("Accountant.Transactions.Filter.AllStatuses");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
