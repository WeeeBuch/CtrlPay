using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend
{
    public static class StatusConverter
    {
        public static StatusEnum ConvertPaymentStatusToFrontendStatus(PaymentStatusEnum paymentStatus)
        {
            return paymentStatus switch
            {
                PaymentStatusEnum.Unpaid => StatusEnum.Pending,
                PaymentStatusEnum.WaitingForPayment => StatusEnum.Pending,
                PaymentStatusEnum.PartiallyPaid => StatusEnum.PartiallyPaid,
                PaymentStatusEnum.Paid => StatusEnum.Completed,
                PaymentStatusEnum.Overpaid => StatusEnum.Completed,
                PaymentStatusEnum.Expired => StatusEnum.Failed,
                PaymentStatusEnum.Cancelled => StatusEnum.Failed,
                _ => StatusEnum.Error,
            };
        }

        public static StatusEnum ConvertTransactionStatusToFrontendStatus(TransactionStatusEnum transactionStatus)
        {
            return transactionStatus switch
            {
                TransactionStatusEnum.Pending => StatusEnum.Pending,
                TransactionStatusEnum.Completed => StatusEnum.Completed,
                TransactionStatusEnum.Failed => StatusEnum.Failed,
                TransactionStatusEnum.Confirmed => StatusEnum.Confirmed,
                _ => StatusEnum.Error,
            };
        }

        public static PaymentStatusEnum ConvertFrontendStatusToPaymentStatus(StatusEnum status)
        {
            return status switch
            {
                StatusEnum.Created => PaymentStatusEnum.Unpaid,
                StatusEnum.WaitingForPayment => PaymentStatusEnum.WaitingForPayment,
                StatusEnum.PartiallyPaid => PaymentStatusEnum.PartiallyPaid,
                StatusEnum.Paid => PaymentStatusEnum.Paid,
                StatusEnum.Overpaid => PaymentStatusEnum.Overpaid,
                StatusEnum.Expired => PaymentStatusEnum.Expired,
                StatusEnum.Cancelled => PaymentStatusEnum.Cancelled,
                _ => PaymentStatusEnum.Unpaid,
            };
        }

        public static TransactionStatusEnum ConvertFrontendStatusToTransactionStatus(StatusEnum status)
        {
            return status switch
            {
                StatusEnum.Pending => TransactionStatusEnum.Pending,
                StatusEnum.Completed => TransactionStatusEnum.Completed,
                StatusEnum.Failed => TransactionStatusEnum.Failed,
                StatusEnum.Confirmed => TransactionStatusEnum.Confirmed,
                _ => TransactionStatusEnum.Pending,
            };
        }
    }
}
