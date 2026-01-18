using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("user_id")]
        private int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column("expires_at_utc")]
        public DateTime ExpiresAtUtc { get; set; }

        [Column("created_at_utc")]
        public DateTime CreatedAtUtc { get; set; }

        [Column("revoked_at_utc")]
        public DateTime? RevokedAtUtc { get; set; } = null;

        // Pomocná property, zda je token stále platný
        [NotMapped]
        public bool IsActive => RevokedAtUtc == null && ExpiresAtUtc > DateTime.UtcNow;
    }
}
