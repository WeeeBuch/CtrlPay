using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class TransactionDTO
    {
        public string Title { get; init; }
        public decimal Amount { get; init; }
        public DateTime Timestamp { get; init; }
        public TransactionStatusEnum State { get; init; }
        public int Id { get; init; }

        public TransactionDTO(string title, decimal amount, DateTime timestamp, TransactionStatusEnum state, int id)
        {
            Title = title;
            Amount = amount;
            Timestamp = timestamp;
            State = state;
            Id = id;
        }

        public TransactionDTO(Transaction transaction)
        {
            Title = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = transaction.Status;
            Id = transaction.Id;
        }
        public TransactionDTO(TransactionApiDTO transaction)
        {
            Title = transaction.TransactionIdXMR.Substring(0,25) + "...";
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = transaction.Status;
            Id = transaction.Id;
        }
        
        public TransactionDTO() { }
    }
    public class PaymentDTO
    {
        public string Title { get; init; }
        public decimal Amount { get; init; }
        public DateTime Timestamp { get; init; }
        public PaymentStatusEnum State { get; init; }
        public PaymentDTO()
        {
            
        }
        public PaymentDTO(PaymentApiDTO payment)
        {
            Title = payment.Id.ToString();
            Amount = payment.PaidAmountXMR;
            Timestamp = payment.CreatedAt;
            State = payment.Status;
        }
    }
}
