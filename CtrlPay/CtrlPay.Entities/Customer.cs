using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("title")]
        public string? Title { get; set; }
        [Column("address")]
        public string? Address { get; set; }
        [Column("postal_code")]
        public string? PostalCode { get; set; }
        [Column("city")]
        public string? City { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("telephone")]
        public string? Phone { get; set; }

    }
}
