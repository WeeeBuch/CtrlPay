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
    public class TransactionRepo
    {

        public static async Task<List<TransactionDTO>> GetTransactions(CancellationToken cancellationToken)
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
            List<TransactionApiDTO> transactions = JsonSerializer.Deserialize<List<TransactionApiDTO>>(json, options);
            List<TransactionDTO> dtoList = new List<TransactionDTO>();

            foreach(var tx in transactions)
            {
                dtoList.Add(new TransactionDTO(tx));
            }

            return dtoList;
        }
    }
}
