using Avalonia.Data.Converters;
using CtrlPay.Avalonia.Translations;
using System;
using System.Globalization;

namespace CtrlPay.Avalonia.Converters;

public class BoolToFlowTableTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFlow)
        {
            // Vracíme přeložený text pomocí tvého TranslationManageru
            return isFlow 
                ? TranslationManager.GetString("Accountant.Transactions.View.Table") 
                : TranslationManager.GetString("Accountant.Transactions.View.Flow");
        }
        return "---";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
