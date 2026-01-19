using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.EntityFrameworkCore;
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
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP error {response.StatusCode}: {body}");
            }

            var rpcResponse = JsonSerializer.Deserialize<RpcResponse<RpcAccountsResult>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rpcResponse?.Error != null)
            {
                throw new Exception($"RPC error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
            }

            RpcAccountsResult accounts = rpcResponse.Result;

            CtrlPayDbContext dbContext = new CtrlPayDbContext();

            foreach (RpcAccount account in accounts.Subaddress_Accounts)
            {
                if (await dbContext.Accounts.FindAsync(account.Account_Index) != null) { continue; }
                var address = dbContext.Addresses.FirstOrDefault(a => a.AddressXMR == account.Base_Address);
                if (address == null)
                {
                    address = new Address(account.Base_Address, true);
                    dbContext.Addresses.Add(address);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                Account newAccount = new Account
                {
                    Index = account.Account_Index,
                    BaseAddress = address
                };
                dbContext.Accounts.Add(newAccount);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
