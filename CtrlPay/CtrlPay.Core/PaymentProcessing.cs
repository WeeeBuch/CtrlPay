using CtrlPay.DB;
using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Core
{
    public class PaymentProcessing
    {
        public static CtrlPayDbContext _db = new CtrlPayDbContext();
        public static async Task PairOneTimePayment(CancellationToken cancellationToken)
        {
            CtrlPayDbContext dbContext = new CtrlPayDbContext();
            
            List<Payment> paymentsWaiting = dbContext.Payments.Where(p => p.Status == PaymentStatusEnum.WaitingForPayment).ToList();

            List<Transaction> transactionsUnpaired = dbContext.Transactions.Where(t => t.Status == TransactionStatusEnum.Confirmed).ToList();

            foreach (var payment in paymentsWaiting)
            {
                var matchingTransactions = transactionsUnpaired.Where(t => t.Address.Id == payment.AddressId && t.Account == payment.Account).ToList();
                if(matchingTransactions == null || matchingTransactions.Count == 0)
                {
                    continue;
                }
                foreach (var transaction in matchingTransactions)
                {
                    payment.PaidAmountXMR += transaction.Amount;
                    if (payment.PaidAmountXMR < payment.ExpectedAmountXMR)
                    {
                        payment.Status = PaymentStatusEnum.PartiallyPaid;
                    }
                    else if (payment.PaidAmountXMR == payment.ExpectedAmountXMR)
                    {
                        payment.Status = PaymentStatusEnum.Paid;
                        payment.PaidAt = DateTime.Now;
                    }
                    else if (payment.PaidAmountXMR > payment.ExpectedAmountXMR)
                    {
                        payment.Status = PaymentStatusEnum.Overpaid;
                        payment.PaidAt = DateTime.Now;
                    }
                    transaction.Payment = payment;
                    transaction.Status = TransactionStatusEnum.Completed;

                }
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }
        public static async Task PayFromCredit(LoyalCustomer customer, Payment pment, CancellationToken cancellationToken)
        {
            Payment payment = _db.Payments.Where(p => p.Id == pment.Id).First();
            decimal amountToPay = payment.ExpectedAmountXMR - payment.PaidAmountXMR;
            decimal credit = _db.Transactions
                .Where(t => t.Account.Index == customer.Account.Index)
                .Where(t => t.Status == TransactionStatusEnum.Completed || t.Status == TransactionStatusEnum.Confirmed)
                .Sum(p => p.Amount);
            decimal ourXmr = _db.LoyalCustomers
                .Where(lc => lc.Account.Index == customer.Account.Index)
                .First()
                .OurXMR;
            credit -= ourXmr;

            if (credit >= amountToPay)
            {
                payment.PaidAmountXMR += amountToPay;
                payment.Status = PaymentStatusEnum.Paid;
                payment.PaidAt = DateTime.Now;
                _db.LoyalCustomers.Where(lc => lc.Account.Index == customer.Account.Index).First().OurXMR += amountToPay;
            }
            else
            {
                payment.PaidAmountXMR += credit;
                payment.Status = PaymentStatusEnum.PartiallyPaid;
                _db.LoyalCustomers.Where(lc => lc.Account.Index == customer.Account.Index).First().OurXMR += credit;
            }
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
