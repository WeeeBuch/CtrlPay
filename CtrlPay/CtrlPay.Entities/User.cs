using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
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
        [Column("loyal_customer_id")]
        private int? LoyalCustomerId { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password_hash")]
        public byte[] PasswordHash { get; set; }
        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }
        [Column("2fa_secret")]
        public byte[]? TwoFactorSecret { get; set; }
        [Column("2fa_enabled")]
        public bool TwoFactorEnabled { get; set; }
        [Column("2fa_recovery_codes")]
        private string? TwoFactorRecoveryCodesJson { get; set; } = string.Empty;

        [NotMapped]
        public string[] TwoFactorRecoveryCodes
        {
            get => string.IsNullOrWhiteSpace(TwoFactorRecoveryCodesJson)
                   ? Array.Empty<string>()
                   : JsonSerializer.Deserialize<string[]>(TwoFactorRecoveryCodesJson)!;
            set => TwoFactorRecoveryCodesJson = JsonSerializer.Serialize(value);
        }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("LoyalCustomerId")]
        public virtual LoyalCustomer? LoyalCustomer { get; set; }

        public User(string username, byte[] passwordHash, byte[] passwordSalt, Role role)
        {
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            RoleId = role.Id;
        }
        public User()
        {
            
        }
    }
}
