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
            /* Udělat metodu, která vrátí transakce podle typu (kredity, čekající platby)+;
             */

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Credentials.JwtAccessToken);
            string uri = $"{Credentials.BaseUri}/api/get_transactions";
            // volání chráněného endpointu
            var response = await httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            List<TransactionApiDTO> transactions = JsonSerializer.Deserialize<List<TransactionApiDTO>>(json);
            List<TransactionDTO> dtoList = new List<TransactionDTO>();

            foreach(var tx in transactions)
            {
                dtoList.Add(new TransactionDTO(tx));
            }

            return dtoList;
        }
    }
}
