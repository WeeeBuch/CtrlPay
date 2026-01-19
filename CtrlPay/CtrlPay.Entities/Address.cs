using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Addresses")]
    public class Address
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("address_xmr")]
        public string AddressXMR { get; set; }
        [Column("is_primary")]
        public bool IsPrimary { get; set; }
        public virtual Account Account { get; set; }

        public Address(string addressXmr, bool isPrimary)
        {
            AddressXMR = addressXmr;
            IsPrimary = isPrimary;
        }
        public Address()
        {
            
        }
    }
}
