using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia.ViewModels;

public partial class QrCodeViewModel : ViewModelBase
{
    [ObservableProperty] private Bitmap? _qrCodeImage;
    [ObservableProperty] private string _address;

    public QrCodeViewModel(string address)
    {
        _address = address;
        // Použijeme pomocnou třídu QrGenerator z předchozí odpovědi
        QrCodeImage = QrGenerator.GenerateBitmap(address);
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        var clipboard = Application.Current?.Clipboard;
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(Address);
            // Zde by bylo fajn mít nějakou notifikaci, 
            // ale pro jednoduchost teď stačí akce.
        }
    }
}
