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
}
