using CtrlPay.Entities;
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
        public static async Task<List<PaymentDTO>> GetPayments(CancellationToken cancellationToken)
        {
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
            List<PaymentDTO> dtoList = new List<PaymentDTO>();

            foreach (var pay in payments)
            {
                dtoList.Add(new PaymentDTO(pay));
            }

            return dtoList;
        }
    }
}
