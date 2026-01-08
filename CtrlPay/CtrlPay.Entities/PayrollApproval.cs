using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("PayrollApprovals")]
    public class PayrollApproval
    {
        [Column("ApproverId")]
        public int ApproverId { get; set; }
        [Column("PayrollId")]
        public int PayrollId { get; set; }
        [Column("Approved")]
        public bool Approved { get; set; }
        [ForeignKey("ApproverId")]
        public virtual Employee Approver { get; set; }
        [ForeignKey("PayrollId")]
        public virtual Payroll Payroll { get; set; }
    }
}
