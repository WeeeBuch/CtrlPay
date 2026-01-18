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

        public TransactionDTO(string title, decimal amount, DateTime timestamp, TransactionStatusEnum state)
        {
            Title = title;
            Amount = amount;
            Timestamp = timestamp;
            State = state;
        }

        public TransactionDTO(Transaction transaction)
        {
            Title = transaction.TransactionIdXMR;
            Amount = transaction.Amount;
            Timestamp = transaction.Timestamp;
            State = transaction.Status;
        }
        public TransactionDTO(TransactionApiDTO transaction)
        {
            Title = transaction.TransactionIdXMR;
            Amount = decimal.Parse(transaction.Amount);
            Timestamp = transaction.Timestamp;
            State = transaction.Status;
        }
        public TransactionDTO() { }
    }
}
