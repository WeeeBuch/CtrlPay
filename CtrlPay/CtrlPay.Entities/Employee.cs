using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        private int UserId { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("personal_number")]
        public string PersonalNumber { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        [Column("reports_to")]
        private int? ReportsToId { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("postal_code")]
        public string PostalCode { get; set; }
        [Column("city")]
        public string City { get; set; }
        [Column("photo")]
        public byte[] Image { get; set; }
        [Column("xmr_address")]
        public string XmrAddress { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(ReportsToId))]
        public virtual Employee? Supervisor { get; set; }
        public virtual List<Employee>? Subordinates { get; set; }
        public virtual List<WorkRecord>? WorkRecords { get; set; }
    }
}
