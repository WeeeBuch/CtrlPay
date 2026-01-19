using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class TransactionApiDTO
    {
        public int Id { get; set; }
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        public string TransactionIdXMR { get; set; }
        public TransactionTypeEnum Type { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PaymentId { get; set; }

        public TransactionApiDTO()
        {
            
        }
        public TransactionApiDTO(int id, int addressId, int accountId, string transactionIdXMR, TransactionTypeEnum type, TransactionStatusEnum status, decimal amount, decimal fee, DateTime timestamp, int? paymentId)
        {
            Id = id;
            AddressId = addressId;
            AccountId = accountId;
            TransactionIdXMR = transactionIdXMR;
            Type = type;
            Status = status;
            Amount = amount;
            Fee = fee;
            Timestamp = timestamp;
            PaymentId = paymentId;
        }

        public TransactionApiDTO(Transaction transaction)
        {
            Id = transaction.Id;
            AddressId = transaction.Address.Id;
            AccountId = transaction.Account.Index;
            TransactionIdXMR = transaction.TransactionIdXMR;
            Type = transaction.Type;
            Status = transaction.Status;
            Amount = transaction.Amount;
            Fee = transaction.Fee;
            Timestamp = transaction.Timestamp;
            if (transaction.Payment != null)
            {
                PaymentId = transaction.Payment.Id;
            }
        }
    }
}
