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
        //TODO: Metody do repos: 2

        public static List<TransactionDTO> GetTransactions(string type)
        {
            /* Udělat metodu, která vrátí transakce podle typu (kredity, čekající platby)
             */

            return [
                new TransactionDTO
                {
                    Title = "Vklad z banky",
                    Amount = 5.0m,
                    Timestamp = DateTime.Now
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1)
                }
                ];
        }

        // DTO = Data Transfer Object
        public record TransactionDTO
        {
            public string Title { get; init; }
            public decimal Amount { get; init; }
            public DateTime Timestamp { get; init; }
        }
    }
}
