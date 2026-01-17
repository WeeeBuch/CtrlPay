using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtrlPay.DB;
using CtrlPay.Entities;

namespace CtrlPay.Core
{
    public class TransactionHandler
    {
        public static async Task PairOneTimePayment(Transaction transaction, CancellationToken cancellationToken)
        {
            CtrlPayDbContext dbContext = new CtrlPayDbContext();
            Address address = transaction.Address;
            Payment? payment = dbContext.Payments.Where(p => p.Status == PaymentStatusEnum.WaitingForPayment && p.Address == address)
                .FirstOrDefault();

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
        }
    }
}
