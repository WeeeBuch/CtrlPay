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
            decimal sum = rnd.Next(0,200);

            return sum;
        }

        public static List<TransactionDTO> GetTransactions(string type)
        {
            /* Udělat metodu, která vrátí transakce podle typu (kredity, čekající platby)
             */

            return [
                new TransactionDTO
                {
                    Title = "Vklad z banky",
                    Amount = 5.0m,
                    Timestamp = DateTime.Now,
                    State = TransactionStatusEnum.Completed
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Completed
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Completed
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2),
                    State = TransactionStatusEnum.Failed
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2),
                    State = TransactionStatusEnum.Failed
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-2),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-10),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                },
                new TransactionDTO
                {
                    Title = "Výplata odměn",
                    Amount = 1.25m,
                    Timestamp = DateTime.Now.AddDays(-1),
                    State = TransactionStatusEnum.Pending
                }
                ];
        }

        // DTO = Data Transfer Object
        public record TransactionDTO
        {
            public string Title { get; init; }
            public decimal Amount { get; init; }
            public DateTime Timestamp { get; init; }
            public TransactionStatusEnum State { get; init; }
        }
    }
}
