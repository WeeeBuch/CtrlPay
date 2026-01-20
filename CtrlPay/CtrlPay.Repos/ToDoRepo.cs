using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class ToDoRepo
    {
        //TODO: Metody do repos: 3

        public static decimal GetTransactionSums(string type)
        {
            /* Udělat metodu co na základě typu vrátí sumu transakcí
             * nebo jze udělat že to bude jen to co má
             */

            Random rnd = new();
            decimal sum = rnd.Next(0,400);

            return sum;
        }

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
}
