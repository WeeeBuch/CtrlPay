using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class FrontendTransactionDTO
    {
        public string? Title { get; init; }
        public decimal Amount { get; init; }
        public DateTime Timestamp { get; init; }
        public StatusEnum State { get; init; }
        public int Id { get; init; }

        public FrontendTransactionDTO(string title, decimal amount, DateTime timestamp, TransactionStatusEnum state, int id)
        {
            Title = title;
            Amount = amount;
            Timestamp = timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(state);
            Id = id;
        }

        public FrontendTransactionDTO(Transaction transaction)
        {
            Title = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(transaction.Status);
            Id = transaction.Id;
        }
        public FrontendTransactionDTO(TransactionApiDTO transaction)
        {
            Title = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(transaction.Status);
            Id = transaction.Id;
        }
        public FrontendTransactionDTO(PaymentApiDTO payment)
        {
            Title = payment.Id.ToString();
            Amount = payment.ExpectedAmountXMR;
            Timestamp = payment.CreatedAt;
            State = StatusConverter.ConvertPaymentStatusToFrontendStatus(payment.Status);
            Id = payment.Id;
        }
        public FrontendTransactionDTO() { }
    }
}
