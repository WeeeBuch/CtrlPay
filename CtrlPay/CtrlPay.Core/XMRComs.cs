using CtrlPay.DB;
using CtrlPay.Entities;
using CtrlPay.XMR;
using System;
using System.Collections.Generic;
using System.Linq;
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
            } else
            {
                addressString = await AccountComs.GenerateOneTimeAddressForLoyalCustomer(customer, httpClient, uri, cancellationToken);
                address = new Address(addressString, false);

                dbContext.Addresses.Add(address);
                var dbPayment = dbContext.Payments.FirstOrDefault(p => p.Id == payment.Id);
                if (dbPayment != null)
                {
                    dbPayment.Address = address;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                return address;
            }

            
        }
    }
}
