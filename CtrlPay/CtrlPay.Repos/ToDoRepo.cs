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
        //TODO: Metody do repos: 4

        public static decimal GetTransactionSums(string type)
        {
            /* Udělat metodu co na základě typu vrátí sumu transakcí
             * nebo jze udělat že to bude jen to co má
             */

            Random rnd = new();
            decimal sum = rnd.Next(0,10);

            return sum;
        }
        public record TransactionDTO
        {
            public string Title { get; init; }
            public decimal Amount { get; init; }
            public DateTime Timestamp { get; init; }
            public TransactionStatusEnum State { get; init; }

            public int Id { get; init; }
        }
        public static void PayFromCredit(TransactionDTO transakce)
        {
            // Implementace platby z kreditu
        }
    }
}
