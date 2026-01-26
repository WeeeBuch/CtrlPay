using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Transactions")]
    public class Transaction
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("address_id")]
        private int AddressId { get; set; }
        [Column("account_id")]
        private int AccountId { get; set; }
        [Column("transaction_xmr_id")]
        public string TransactionIdXMR { get; set; }
        [Column("type")]
        public TransactionTypeEnum Type { get; set; }
        [Column("status")]
        public TransactionStatusEnum Status { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("fee")]
        public decimal Fee { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("payment_id")]
        private int? PaymentId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public Transaction()
        {
            
        }

        public Transaction(TransactionApiDTO dto)
        {
            Id = dto.Id;
            AddressId = dto.AddressId;
            AccountId = dto.AccountId;
            TransactionIdXMR = dto.TransactionIdXMR;
            Type = dto.Type;
            Status = dto.Status;
            Amount = dto.Amount;
            Fee = dto.Fee;
            Timestamp = dto.Timestamp;
            PaymentId = dto.PaymentId;
        }
    }
}
