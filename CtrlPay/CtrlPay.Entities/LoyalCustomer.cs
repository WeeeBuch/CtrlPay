using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("LoyalCustomers")]
    public class LoyalCustomer
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("customer_id")]
        public int? CustomerId { get; set; }
        [Column("account_id")]
        private int AccountId { get; set; }
        [Column("our_xmr")]
        public decimal OurXMR { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public virtual List<User> Users { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
