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
        public string? SubTitle { get; init; }
        public decimal Amount { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public StatusEnum State { get; init; }
        public int Id { get; init; }

        public FrontendTransactionDTO(string title, decimal amount, DateTimeOffset timestamp, TransactionStatusEnum state, int id)
        {
            Title = title;
            SubTitle = id.ToString();
            Amount = amount;
            Timestamp = timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(state);
            Id = id;
        }

        public FrontendTransactionDTO(Transaction transaction)
        {
            Title = string.IsNullOrEmpty(transaction.TransactionIdXMR) ? "Transaction" : transaction.TransactionIdXMR;
            SubTitle = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(transaction.Status);
            Id = transaction.Id;
        }
        public FrontendTransactionDTO(TransactionApiDTO transaction)
        {
            Title = string.IsNullOrEmpty(transaction.TransactionIdXMR) ? "Transaction" : transaction.TransactionIdXMR;
            SubTitle = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = StatusConverter.ConvertTransactionStatusToFrontendStatus(transaction.Status);
            Id = transaction.Id;
        }
        public FrontendTransactionDTO(PaymentApiDTO payment)
        {
            Title = string.IsNullOrEmpty(payment.Title) ? $"Payment #{payment.Id}" : payment.Title;
            SubTitle = $"#{payment.Id}";
            Amount = payment.ExpectedAmountXMR;
            Timestamp = payment.CreatedAt;
            State = StatusConverter.ConvertPaymentStatusToFrontendStatus(payment.Status);
            Id = payment.Id;
        }
        public FrontendTransactionDTO() { }
    }
}
