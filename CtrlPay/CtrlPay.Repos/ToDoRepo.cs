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
    //TODO: Metody do repos: 2

    public static void PayFromCredit(FrontendTransactionDTO payment)
    {
        // Implementace platby z kreditu
        AppLogger.Info($"Paying from credit...");
    }
}
