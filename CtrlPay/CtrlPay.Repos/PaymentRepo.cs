using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class PaymentRepo
    {
        public static async Task<List<TransactionDTO>> GetPayments(CancellationToken cancellationToken)
        {
            return [
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
    }
}
