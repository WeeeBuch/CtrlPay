using CtrlPay.Avalonia.Settings;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia;

public static class ChangeChecker
{
    public static async void ToCheck() 
    {
        AppLogger.Info($"Checking....");
        await TransactionRepo.UpdateTransactionsCacheFromApi(default);
        await TransactionRepo.UpdateTransactionSumCacheFromApi(default);
        await PaymentRepo.UpdatePaymentSumCacheFromApi(default);
        await PaymentRepo.UpdatePaymetsCacheFromApi(default);
        // Sem se píšou všechny kontroly změn, které chceme provádět
        UpdateHandler.HandleCreditAvailableUpdate(TransactionRepo.GetTransactionSum());
        UpdateHandler.HandlePendingPaymentsUpdate(PaymentRepo.GetPaymentSum());
        UpdateHandler.HandleNewDebtsAdded();
        UpdateHandler.HandleNewPaymentsAdded();

        Credentials.BaseUri = SettingsManager.Current.ConnectionString;
        AppLogger.Info($"Checking completed.");
    }

    public async static Task StartChecking(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                ToCheck();
                // Počkáme zvolený interval před dalším spuštěním
                await Task.Delay(TimeSpan.FromSeconds(SettingsManager.Current.RefreshRate), ct);
            }
            catch (TaskCanceledException) { break; } // Normální ukončení
            catch (Exception ex)
            {
                // Tady můžeš zalogovat chybu, aby ti nespadla celá smyčka
                AppLogger.Error($"Checker failed.", ex);
            }
        }
    }
}
