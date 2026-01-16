using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.XMR
{
    public class AccountComs
    {
        public static async Task Synchronize(HttpClient httpClient,string uri, CancellationToken cancellationToken)
        {
            var payload = new
            {
                jsonrpc = "2.0",
                id = "0",
                method = "get_accounts"
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await httpClient.PostAsync(
                uri,
                content,
                cancellationToken
            );

            string body = await response.Content.ReadAsStringAsync();

            var rpcResponse = JsonSerializer.Deserialize<RpcResponse<GetAccountsResult>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rpcResponse.Error != null)
            {
                throw new Exception($"RPC error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
            }

            var accounts = rpcResponse.Result;
        }
    }
}
