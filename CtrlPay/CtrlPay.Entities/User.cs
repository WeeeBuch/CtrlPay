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
    [Table("Users")]
    public class User
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("role_id")]
        private int RoleId { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("2fa_secret")]
        public string TwoFactorSecret { get; set; }
        [Column("2fa_enabled")]
        public bool TwoFactorEnabled { get; set; }
        [Column("2fa_recovery_code")]
        public string TwoFactorRecoveryCode { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
