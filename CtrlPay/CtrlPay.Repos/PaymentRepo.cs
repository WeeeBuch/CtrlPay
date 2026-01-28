using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class PaymentRepo
    {
        private static List<FrontendTransactionDTO> PaymentsCache = [];
        private static DateTime LastPaymentsCacheUpdate = DateTime.MinValue;
        private static decimal PaymentSumCache = 0;
        private static DateTime LastPaymentSumCacheUpdate = DateTime.MinValue;
        private static string SortMethod = "DateDesc";
        public static async Task UpdatePaymetsCacheFromApi(CancellationToken cancellationToken)
        {
            #region Debug
            if (DebugMode.MockPayments)
            {
                PaymentsCache = GetPayments();
                return;
            }
            #endregion

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string json = await HttpGetter.HttpGet("/api/payments/my");

            // Přidej options do metody Deserialize
            PaymentsCache = [];
            JsonSerializer.Deserialize<ReturnModel<List<PaymentApiDTO>>>(json, options).Body.ForEach(p => PaymentsCache.Add(new(p)));
            
            LastPaymentsCacheUpdate = DateTime.UtcNow;
        }
        public static List<FrontendTransactionDTO> GetPayments()
        {
            #region Debug
            if (DebugMode.MockPayments)
            {
                List<FrontendTransactionDTO> debugList = [
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 1",
                        Amount = 123.45m,
                        Timestamp = DateTime.UtcNow.AddDays(-1),
                        State = StatusEnum.Completed,
                        Id = 1
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 2",
                        Amount = 67.89m,
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        State = StatusEnum.Pending,
                        Id = 2
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 3",
                        Amount = 10.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-3),
                        State = StatusEnum.Failed,
                        Id = 3
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 4",
                        Amount = 250.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-4),
                        State = StatusEnum.Confirmed,
                        Id = 4
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 5",
                        Amount = 75.50m,
                        Timestamp = DateTime.UtcNow.AddDays(-5),
                        State = StatusEnum.Paid,
                        Id = 5
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Debt 6",
                        Amount = 300.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-6),
                        State = StatusEnum.Expired,
                        Id = 6
                    }
                ];
                return debugList;
            }
            #endregion
            return PaymentsCache;
        }
        public static async Task UpdatePaymentSumCacheFromApi(CancellationToken cancellationToken)
        {
            if (DebugMode.MockPaymentSum)
            {
                Random rnd = new();
                PaymentSumCache = rnd.Next(0,500);
                return;
            }

            // Definuj si options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string json = await HttpGetter.HttpGet("/api/payments/amount-due");

            decimal suma = JsonSerializer.Deserialize<ReturnModel<decimal>>(json, options).Body;

            PaymentSumCache = suma;
            LastPaymentSumCacheUpdate = DateTime.UtcNow;
        }

        

        public static decimal GetPaymentSum() => PaymentSumCache;

        public static List<FrontendTransactionDTO> GetSortedDebts(string? sortingMethod, bool payable)
        {
            if (sortingMethod != null) SortMethod = sortingMethod;

            string sortMethod = sortingMethod ?? SortMethod;

            List<FrontendTransactionDTO> filteredData = 
            [..
                PaymentsCache.Where(t => !payable || t.Amount <= TransactionRepo.GetTransactionSum())
            ];

            return sortMethod switch
            {
                "AmountAsc" => [.. filteredData.OrderBy(d => d.Amount)],
                "AmountDesc" => [.. filteredData.OrderByDescending(d => d.Amount)],
                "DateAsc" => [.. filteredData.OrderBy(d => d.Timestamp)],
                "DateDesc" => [.. filteredData.OrderByDescending(d => d.Timestamp)],
                _ => [.. filteredData.OrderBy(d => d.Timestamp)]
            }; ;
        }
    }
}
