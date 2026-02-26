using CtrlPay.DB;
using CtrlPay.Entities;
using CtrlPay.XMR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Core
{
    public static class XMRComs
    {
        public static async Task SynchronizeAccounts(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            await AccountComs.Synchronize(httpClient, uri, cancellationToken);
        }
        public static async Task LookForNewTransactions(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            await TransactionComs.LookForNew(httpClient, uri, cancellationToken);
        }
        public static async Task ConfirmPendingTransactions(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            await TransactionComs.ConfirmPending(httpClient, uri, cancellationToken);
        }
        public static async Task<Address> GenerateOneTimeAddressForLoyalCustomer(LoyalCustomer customer, Payment payment, HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            Address address;
            string addressString;
            CtrlPayDbContext dbContext = new CtrlPayDbContext();

            Payment paymentFromDb = dbContext.Payments.FirstOrDefault(p => p.Id == payment.Id);
            if (paymentFromDb == null)
            {
                throw new Exception("Payment not found in database.");
            }
            if (paymentFromDb.Address != null)
            {
                addressString = paymentFromDb.Address.AddressXMR;
                address = new Address(addressString, false);
                return address;
            }
            else
            {
                addressString = await AccountComs.GenerateOneTimeAddressForLoyalCustomer(customer, httpClient, uri, cancellationToken);
                address = new Address(addressString, false);

                dbContext.Addresses.Add(address);
                var dbPayment = dbContext.Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (dbPayment != null)
                {
                    dbPayment.Address = address;
                }
                dbPayment.Status = PaymentStatusEnum.WaitingForPayment;
                await dbContext.SaveChangesAsync(cancellationToken);
                return address;
            }
        }
        public static async Task PromoteCustomer(int customerId, MoneroRpcOptions _rpcOptions)
        {
            CancellationToken cancellationToken = new CancellationToken();
            string username = _rpcOptions.Username;
            string password = _rpcOptions.Password;
            string uri = $"http://{_rpcOptions.Host}:{_rpcOptions.Port}/json_rpc";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                PreAuthenticate = false, // u Digest MUSÍ být false
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            CtrlPayDbContext dbContext = new CtrlPayDbContext();
            Customer customer = dbContext.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found in database.");
            }
            int newAccountIndex = await AccountComs.CreateNewAccount(httpClient, uri, cancellationToken);
            await AccountComs.Synchronize(httpClient, uri, cancellationToken);
            LoyalCustomer newLoyal = new LoyalCustomer()
            {
                Customer = customer,
                Account = dbContext.Accounts.FirstOrDefault(a => a.Index == newAccountIndex) ?? throw new Exception("New account not found in database."),
                OurXMR = 0
            };
            dbContext.LoyalCustomers.Add(newLoyal);
            await dbContext.SaveChangesAsync();
        }

    }
}
