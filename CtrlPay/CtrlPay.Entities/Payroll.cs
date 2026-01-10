using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("Payrolls")]
    public class Payroll
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("employee_id")]
        private int EmployeeId { get; set; }
        [Column("amount_xmr")]
        public decimal AmountXMR { get; set; }
        [Column("period_from")]
        public DateTime PeriodFrom { get; set; }
        [Column("period_to")]
        public DateTime PeriodTo { get; set; }
        [Column("paid_at")]
        public DateTime PaidAt { get; set; }
        [Column("status")]
        public PayrollStatusEnum Status { get; set; }
        [Column("total_hours")]
        public int TotalHours { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public virtual List<PayrollApproval> Approvals { get; set; }


    }
}
