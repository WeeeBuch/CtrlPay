using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("WorkRecords")]
    public class WorkRecord
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("employee_id")]
        private int EmployeeId { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("worked_hours")]
        public int HoursWorked { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
