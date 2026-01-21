using CtrlPay.Repos;
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
        await TransactionRepo.UpdateTransactionsCacheFromApi(default);
        await TransactionRepo.UpdateTransactionSumCacheFromApi(default);
        await PaymentRepo.UpdatePaymentSumCacheFromApi(default);
        await PaymentRepo.UpdatePaymetsCacheFromApi(default);
        // Sem se píšou všechny kontroly změn, které chceme provádět
        UpdateHandler.HandleCreditAvailableUpdate(TransactionRepo.GetTransactionSum());
        UpdateHandler.HandlePendingPaymentsUpdate(PaymentRepo.GetPaymentSum());
    }

    public async static Task StartChecking(TimeSpan interval, CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                ToCheck();
                // Počkáme zvolený interval před dalším spuštěním
                await Task.Delay(interval, ct);
            }
            catch (TaskCanceledException) { break; } // Normální ukončení
            catch (Exception ex)
            {
                // Tady můžeš zalogovat chybu, aby ti nespadla celá smyčka
            }
        }
    }
}
