using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.XMR
{
    public class TransactionComs
    {
        public static async Task LookForNew(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            var payload = new
            {
                jsonrpc = "2.0",
                id = "0",
                method = "get_transfers",
                @params = new
                {
                    @in = true,
                    all_accounts = true
                }
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

            var rpcResponse = JsonSerializer.Deserialize<RpcResponse<RpcTransfersResult>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rpcResponse?.Error != null)
            {
                throw new Exception($"RPC error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
            }

            RpcTransfersResult transfers = rpcResponse.Result;
            List<Transaction> transactions = new List<Transaction>();
            CtrlPayDbContext dbContext = new CtrlPayDbContext();


            transfers.In.ForEach(async t =>
            {
                if(dbContext.Addresses.Any(a => a.AddressXMR == t.Address) == false)
                {
                    dbContext.Addresses.Add(new Address
                    {
                        AddressXMR = t.Address,
                        IsPrimary = t.Subaddr_Index.Minor == 0
                    });
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                transactions.Add(new Transaction
                {
                    Address = dbContext.Addresses.FirstOrDefault(a => a.AddressXMR == t.Address),
                    TransactionIdXMR = t.Txid,
                    Type = TransactionTypeEnum.In,
                    Status = t.Locked ? TransactionStatusEnum.Pending : TransactionStatusEnum.Confirmed,
                    Amount = t.Amount / 1_000_000_000_000m,
                    Fee = t.Fee,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(t.Timestamp).DateTime,
                    Account = dbContext.Accounts.FirstOrDefault(a => a.Index == t.Subaddr_Index.Major)
                });
            });

            foreach (Transaction tx in transactions)
            {
                
                if (tx.Account.Index == 0) { continue; }
                if (dbContext.Transactions.Any(t => t.TransactionIdXMR == tx.TransactionIdXMR))
                {
                    continue;
                }
                else
                {
                    dbContext.Transactions.Add(tx);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            
        }
        public static async Task ConfirmPending(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            CtrlPayDbContext dbContext = new CtrlPayDbContext();
            List<Transaction> unconfirmedTransactions = dbContext.Transactions
                .Where(t => t.Status == TransactionStatusEnum.Pending)
                .ToList();

            foreach (Transaction tx in unconfirmedTransactions)
            {
                Transaction updatedTx = await GetTransactionByTxId(httpClient, uri, tx.TransactionIdXMR, cancellationToken, dbContext);
                if (updatedTx.Status == TransactionStatusEnum.Confirmed)
                {
                    tx.Status = TransactionStatusEnum.Confirmed;
                }
            }
        }
        public static async Task<Transaction> GetTransactionByTxId(HttpClient httpClient, string uri, string txId, CancellationToken cancellationToken, CtrlPayDbContext dbContext)
        {
            var payload = new
            {
                jsonrpc = "2.0",
                id = "0",
                method = "get_transfers",
                @params = new
                {
                    txId = txId
                }
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

            var rpcResponse = JsonSerializer.Deserialize<RpcResponse<RpcTransfer>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rpcResponse?.Error != null)
            {
                throw new Exception($"RPC error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
            }

            RpcTransfer transfer = rpcResponse.Result;

            return new Transaction
            {
                Address = dbContext.Addresses.FirstOrDefault(a => a.AddressXMR == transfer.Address),
                TransactionIdXMR = transfer.Txid,
                Type = TransactionTypeEnum.In,
                Status = transfer.Locked ? TransactionStatusEnum.Pending : TransactionStatusEnum.Confirmed,
                Amount = transfer.Amount / 1_000_000_000_000m,
                Fee = transfer.Fee,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(transfer.Timestamp).DateTime,
                Account = dbContext.Accounts.FirstOrDefault(a => a.Index == transfer.Subaddr_Index.Major)
            };
        }   
    }
}
