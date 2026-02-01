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

        #region Debug
        if (DebugMode.SkipApiConnectionTest)
        {
            await Task.Delay(2000); // Zkrátil jsem na 2s, ať nečekáš věčnost
            return !string.IsNullOrWhiteSpace(connString);
        }
        #endregion

        // Dočasně nastavíme BaseUri pro worker, aby věděl, kam volat
        // Pokud tvůj HttpWorker používá statické Credentials.BaseUri, musíme ho pro test nastavit
        string originalBaseUri = Credentials.BaseUri;
        Credentials.BaseUri = connString;

        try
        {
            // Použijeme tvůj HttpGet z workeru. 
            // Cesta je "/health/api" (worker si ji sám spojí s BaseUri)
            var result = await HttpWorker.HttpGet("/health/api");

            // Pokud worker vrátil null, spojení selhalo (catch block v HttpWorkeru)
            if (result == null)
            {
                AppLogger.Error($"Connection test failed for: {connString}");
                return false;
            }

            AppLogger.Info($"Successfully tested connection to: {connString}");
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Failed to test connection to {connString}", ex);
            return false;
        }
        finally
        {
            // Vrátíme původní BaseUri zpět, aby se nic nerozbilo ve zbytku appky
            // (V onboardingu to sice asi nevadí, ale je to slušnost)
            Credentials.BaseUri = originalBaseUri;
        }
    }
}
