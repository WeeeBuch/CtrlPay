using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtrlPay.Entities;
using CtrlPay.Repos;
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
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _copyString;

    public QrCodeViewModel(string address, FrontendTransactionDTO transaction)
    {
        Address = address;
        Title = transaction.Title;
        QrCodeImage = GenQR(transaction);
    }

    private Bitmap GenQR(FrontendTransactionDTO tx)
    {
        MoneroTransaction generator = new(Address, (float)tx.Amount, tx.Id.ToString(), "You", tx.Title);
        string payload = generator.ToString();
        CopyString = payload;

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
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var activeWindow = desktop.Windows.FirstOrDefault(w => w.IsActive)
                               ?? desktop.MainWindow
                               ?? desktop.Windows.FirstOrDefault();

            var clipboard = activeWindow?.Clipboard;

            if (clipboard != null)
            {
                await clipboard.SetTextAsync(CopyString);

                ShowCopyMessage = true;
                await Task.Delay(2000);
                ShowCopyMessage = false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Kritická chyba: Schránka nebyla nalezena v žádném okně.");
            }
        }
    }
}
