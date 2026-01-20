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
                return new List<FrontendTransactionDTO>
                {
                    new FrontendTransactionDTO
                    {
                        Title = "Testovací platba 1",
                        Amount = 0.5m,
                        Timestamp = DateTime.UtcNow.AddHours(-5),
                        State = StatusEnum.Paid,
                        Id = 1
                    },
                    new FrontendTransactionDTO
                    {
                        Title = "Testovací platba 2 ale moc dlouhá snad se mi zalomí nebo zkrátí.",
                        Amount = 1.2m,
                        Timestamp = DateTime.UtcNow.AddHours(-2),
                        State = StatusEnum.Pending,
                        Id = 2
                    }
                };
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
