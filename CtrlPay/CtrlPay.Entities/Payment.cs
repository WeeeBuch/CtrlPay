using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("customer_id")]
        public int? CustomerId { get; set; }
        [Column("account_id")]
        public int? AccountId { get; set; }
        [Column("address_id")]
        public int? AddressId { get; set; }
        [Column("expected_xmr_amount")]
        public decimal ExpectedAmountXMR { get; set; }
        [Column("paid_xmr_amount")]
        public decimal PaidAmountXMR { get; set; }
        [Column("status")]
        public PaymentStatusEnum Status { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column("paid_at")]
        public DateTimeOffset? PaidAt { get; set; }
        [Column("due_date")]
        public DateTimeOffset? DueDate { get; set; }
        [Column("title")]
        public string? Title { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }
        //public virtual List<Transaction> Transactions { get; set; }

        public Payment()
        {
            
        }

        public Payment(PaymentApiDTO dto)
        {
            Id = dto.Id;
            CustomerId = dto.CustomerId;
            AccountId = dto.AccountId;
            AddressId = dto.AddressId;
            ExpectedAmountXMR = dto.ExpectedAmountXMR;
            PaidAmountXMR = dto.PaidAmountXMR;
            Status = dto.Status;
            CreatedAt = dto.CreatedAt;
            PaidAt = dto.PaidAt ?? default;
            DueDate = dto.DueDate ?? default;
        }

    }
}
