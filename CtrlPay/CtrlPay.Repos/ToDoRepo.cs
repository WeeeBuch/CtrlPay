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
    }

    public static string GetOneTimeAddress(FrontendTransactionDTO transaction)
    {
        // Implementace generování jednorázové adresy
        // transakce pro pozdější automatické napojení
        return "generated_one_time_address";
    }

    public static async Task<bool> TestConnectionToAPI(string connString)
    {
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
            response = await client.GetAsync(uri);
        }
        catch (Exception)
        {
            return false;
        }

        if (response == null)
        {
            return false;
        }

        return true;
    }
}
