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
        private static List<TransactionApiDTO> Transactions { get; set; } = new List<TransactionApiDTO>();
        private static DateTime LastUpdatedTransactions { get; set; } = DateTime.MinValue;
        private static decimal TransactionSumCache { get; set; } = 0;
        private static DateTime LastUpdatedTransactionSum { get; set; } = DateTime.MinValue;
        private static async Task GetTransactionsFromApiIfNeeded(CancellationToken cancellationToken)
        {
            

            if ((DateTime.UtcNow - LastUpdatedTransactions).TotalMinutes < 2) { return; }

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            string uri = $"{Credentials.BaseUri}/api/transactions/my";
            // volání chráněného endpointu
            var response = await httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            // Definuj si options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string json = await response.Content.ReadAsStringAsync();

            // Přidej options do metody Deserialize
            List<TransactionApiDTO> transactionsDtos = JsonSerializer.Deserialize<List<TransactionApiDTO>>(json, options);
            Transactions = transactionsDtos;
            LastUpdatedTransactions = DateTime.UtcNow;
        }
        public static async Task<List<FrontendTransactionDTO>> GetTransactions(CancellationToken cancellationToken)
        {
            #region Debug
            if (DebugMode.IsDebugMode)
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
            }


            #endregion
            await GetTransactionsFromApiIfNeeded(cancellationToken);
            return Transactions.Select(t => new FrontendTransactionDTO(t)).ToList();
        }
        private static async Task GetTransactionSumFromApiIfNeeded(CancellationToken cancellationToken)
        {
            if ((DateTime.UtcNow - LastUpdatedTransactionSum).TotalMinutes < 2) { return; }

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            string uri = $"{Credentials.BaseUri}/api/transactions/credit";
            // volání chráněného endpointu
            var response = await httpClient.GetAsync(uri, cancellationToken);

            response.EnsureSuccessStatusCode();
            // Definuj si options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            decimal suma = decimal.Parse(await response.Content.ReadAsStringAsync(), System.Globalization.CultureInfo.InvariantCulture);

            TransactionSumCache = suma;
            LastUpdatedTransactionSum = DateTime.UtcNow;
        }
        public static async Task<decimal> GetTransactionSum(CancellationToken cancellationToken)
        {
            await GetTransactionSumFromApiIfNeeded(cancellationToken);
            return TransactionSumCache;
        }
    }
}
