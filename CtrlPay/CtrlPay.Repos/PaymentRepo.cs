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
    public static class PaymentRepo
    {
        private static List<PaymentApiDTO> PaymentsCache = new List<PaymentApiDTO>();
        private static DateTime LastPaymentsCacheUpdate = DateTime.MinValue;
        private static decimal PaymentSumCache = 0;
        private static DateTime LastPaymentSumCacheUpdate = DateTime.MinValue;
        private static async Task GetPaymentsFromApiIfNeeded(CancellationToken cancellationToken)
        {
            if ((DateTime.UtcNow - LastPaymentSumCacheUpdate).TotalMinutes < 2) { return; }

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            string uri = $"{Credentials.BaseUri}/api/payments/my";
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
            List<PaymentApiDTO> payments = JsonSerializer.Deserialize<List<PaymentApiDTO>>(json, options);
            PaymentsCache = payments;
            LastPaymentsCacheUpdate = DateTime.UtcNow;
        }
        public static async Task<List<FrontendTransactionDTO>> GetPayments(CancellationToken cancellationToken)
        {
            #region Debug
            if (DebugMode.IsDebugMode)
            {
                await Task.Delay(500, cancellationToken); // Simulace zpoždění
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
            await GetPaymentsFromApiIfNeeded(cancellationToken);
            return PaymentsCache.Select(p => new FrontendTransactionDTO(p)).ToList();

        }
        private static async Task GetPaymentSumFromApiIfNeeded(CancellationToken cancellationToken)
        {
            if ((DateTime.UtcNow - LastPaymentSumCacheUpdate).TotalMinutes < 2) { return; }

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            string uri = $"{Credentials.BaseUri}/api/payments/amount-due";
            // volání chráněného endpointu
            var response = await httpClient.GetAsync(uri, cancellationToken);

            response.EnsureSuccessStatusCode();
            // Definuj si options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            decimal suma = decimal.Parse(await response.Content.ReadAsStringAsync(), System.Globalization.CultureInfo.InvariantCulture);

            PaymentSumCache = suma;
            LastPaymentSumCacheUpdate = DateTime.UtcNow;
        }

        public static async Task<decimal> GetPaymentSum(CancellationToken cancellationToken)
        {
            await GetPaymentsFromApiIfNeeded(cancellationToken);
            return PaymentSumCache;
        }
    }
}
