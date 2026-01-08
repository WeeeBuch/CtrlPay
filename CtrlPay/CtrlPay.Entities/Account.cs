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
    [Table("Accounts")]
    public class Account
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("index")]
        public int Index { get; set; }
        [Column("base_address_xmr")]
        private int BaseAddressId { get; set; }
        [ForeignKey("BaseAddressId")]
        public virtual Address BaseAddress { get; set; }
        //public virtual List<Address> Addresses { get; set; }
    }
}
