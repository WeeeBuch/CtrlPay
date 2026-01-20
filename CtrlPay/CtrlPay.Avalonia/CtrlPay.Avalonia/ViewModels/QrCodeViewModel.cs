using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace CtrlPay.Avalonia.ViewModels;

public partial class QrCodeViewModel : ViewModelBase
{
    [ObservableProperty] private Bitmap? _qrCodeImage;
    [ObservableProperty] private string _address;
    [ObservableProperty] private bool _showCopyMessage = false;

    public QrCodeViewModel(string address)
    {
        Address = address;
        QrCodeImage = GenQR();
    }

    private Bitmap GenQR()
    {
        MoneroTransaction generator = new(Address);
        string payload = generator.ToString();

        QRCodeGenerator qrGenerator = new();
        QRCodeData qRCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new(qRCodeData);

        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        using MemoryStream ms = new(qrCodeBytes);
        return new Bitmap(ms);
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);

        if (topLevel?.Clipboard is { } clipboard)
        {
            await clipboard.SetTextAsync(Address);

            ShowCopyMessage = true;
            await Task.Delay(2000);
            ShowCopyMessage = false;
        }
    }
}
