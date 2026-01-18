using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class JwtAuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";

        public int ExpiresIn { get; set; }                // v sekundách
        public DateTime ExpiresAtUtc { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
        public bool IsTotp { get; set; } = false;
    }
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;

        // RSA private key (PEM)
        public string PrivateKeyPem { get; set; } = string.Empty;
    }


}
