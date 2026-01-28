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
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class TransactionRepo
    {
        private static List<FrontendTransactionDTO> TransactionsCache { get; set; } = [];
        private static DateTime LastUpdatedTransactions { get; set; } = DateTime.MinValue;
        private static decimal TransactionSumCache { get; set; } = 0;
        private static DateTime LastUpdatedTransactionSum { get; set; } = DateTime.MinValue;
        private static string SortMethod = "DateDesc";

        public static async Task UpdateTransactionsCacheFromApi(CancellationToken cancellationToken)
        {
            #region Debug
            if (DebugMode.MockTransactions)
            {
                TransactionsCache = GetTransactions();
                return;
            }
            #endregion

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            // Definuj si options
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            string json = await HttpGetter.HttpGet("/api/transactions/my");

            // Přidej options do metody Deserialize
            JsonSerializer.Deserialize<ReturnModel<List<TransactionApiDTO>>>(json, options).Body.ForEach(t => TransactionsCache.Add(new(t)));
            LastUpdatedTransactions = DateTime.UtcNow;
        }
        public static List<FrontendTransactionDTO> GetTransactions()
        {
            #region Debug
            if (DebugMode.MockTransactions)
            {
                List<FrontendTransactionDTO> debugList = [
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 1",
                        Amount = 123.45m,
                        Timestamp = DateTime.UtcNow.AddDays(-1),
                        State = StatusEnum.Completed,
                        Id = 1
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 2",
                        Amount = 67.89m,
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        State = StatusEnum.Pending,
                        Id = 2
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 3",
                        Amount = 10.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-3),
                        State = StatusEnum.Failed,
                        Id = 3
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 4",
                        Amount = 250.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-4),
                        State = StatusEnum.Confirmed,
                        Id = 4
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 5",
                        Amount = 75.50m,
                        Timestamp = DateTime.UtcNow.AddDays(-5),
                        State = StatusEnum.Paid,
                        Id = 5
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Debug Transaction 6",
                        Amount = 300.00m,
                        Timestamp = DateTime.UtcNow.AddDays(-6),
                        State = StatusEnum.Expired,
                        Id = 6
                    }
                ];
                return debugList;
            }
            #endregion
            return TransactionsCache;
        }

        public static async Task UpdateTransactionSumCacheFromApi(CancellationToken cancellationToken)
        {
            #region Debug
            if (DebugMode.MockTransactionSum)
            {
                Random rnd = new();
                TransactionSumCache = rnd.Next(0, 500);
                return;
            }
            #endregion

            string json = await HttpGetter.HttpGet("/api/transactions/credit");

            //TODO: Změnit na deserializaci ReturnModelu
            decimal suma = decimal.Parse(json, System.Globalization.CultureInfo.InvariantCulture);
            TransactionSumCache = suma;
            LastUpdatedTransactionSum = DateTime.UtcNow;
        }

        public static decimal GetTransactionSum() => TransactionSumCache;

        public static List<FrontendTransactionDTO> GetSortedTransactions(string? sortingMethod)
        {
            if (sortingMethod != null) SortMethod = sortingMethod;

            string sortMethod = sortingMethod ?? SortMethod;

            return sortMethod switch
            {
                "AmountAsc" => [.. TransactionsCache.OrderBy(d => d.Amount)],
                "AmountDesc" => [.. TransactionsCache.OrderByDescending(d => d.Amount)],
                "DateAsc" => [.. TransactionsCache.OrderBy(d => d.Timestamp)],
                "DateDesc" => [.. TransactionsCache.OrderByDescending(d => d.Timestamp)],
                _ => [.. TransactionsCache.OrderBy(d => d.Timestamp)]
            }; ;
        }
    }
}
