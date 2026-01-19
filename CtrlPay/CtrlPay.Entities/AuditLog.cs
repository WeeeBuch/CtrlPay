using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("state")]
        public AuditLogStateEnum State { get; set; }
        [Column("severity")]
        public AuditLogSeverityEnum Severity { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
