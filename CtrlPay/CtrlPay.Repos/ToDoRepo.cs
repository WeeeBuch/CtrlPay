using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public static class ToDoRepo
{
    //TODO: Metody do repos: 3

    public static void PayFromCredit(FrontendTransactionDTO transakce)
    {
        // Implementace platby z kreditu
        AppLogger.Info($"Paying from credit...");
    }

    public static string GetOneTimeAddress(FrontendTransactionDTO transaction)
    {
        // Implementace generování jednorázové adresy
        // transakce pro pozdější automatické napojení
        AppLogger.Info($"Getting onetime address...");
        return "generated_one_time_address";
    }

    public static async Task<bool> TestConnectionToAPI(string connString)
    {
        AppLogger.Info($"Testing connection to: {connString}");
        // Tady se testne konekce a pokud je úspěšná tak se vrátí true jinak false

        #region Debug
        if (DebugMode.SkipApiConnectionTest)
        {
            await Task.Delay(5000);

            return string.IsNullOrWhiteSpace(connString);
        }
        #endregion

        var handler = new HttpClientHandler
        {
            UseProxy = false
        };

        using HttpClient client = new(handler);
        string uri = $"{connString}/health/api";

        HttpResponseMessage response;
        try
        {
            AppLogger.Info($"Getting health response from API...");
            response = await client.GetAsync(uri);
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Failed to test connection.", ex);
            return false;
        }

        if (response == null)
        {
            AppLogger.Error($"Returned response was NULL.");
            return false;
        }

        AppLogger.Info($"Succesfully tested connection to: {connString}");
        return true;
    }
}
