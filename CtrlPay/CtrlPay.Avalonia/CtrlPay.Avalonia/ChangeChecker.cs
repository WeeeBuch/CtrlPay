using CtrlPay.Avalonia.Settings;
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
    public static void ToCheck() 
    {
        // Sem se píšou všechny kontroly změn, které chceme provádět
        UpdateHandler.HandleCreditAvailableUpdate(ToDoRepo.GetTransactionSums("credits"));
        UpdateHandler.HandlePendingPaymentsUpdate(ToDoRepo.GetTransactionSums("pendings"));
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
            catch (Exception)
            {
                // Tady můžeš zalogovat chybu, aby ti nespadla celá smyčka
            }
        }
    }
}
