using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    internal static class Credentials
    {
        public static string JwtAccessToken { get; set; }
        public static string BaseUri { get; set; } = "https://localhost:5000";
        public static DateTime AccessTokenExpiresAtUtc { get; set; }
        public static string RefreshToken { get; set; }
        public static DateTime RefreshTokenExpiresAtUtc { get; set; }


        public static void Clear()
        {
            JwtAccessToken = string.Empty;
            AccessTokenExpiresAtUtc = DateTime.MinValue;
            RefreshToken = string.Empty;
            RefreshTokenExpiresAtUtc = DateTime.MinValue;
        }
        public static bool IsTokenActive()
        {
            return !string.IsNullOrEmpty(JwtAccessToken) && AccessTokenExpiresAtUtc > DateTime.UtcNow;
        }
        public static void RefreshTokens()
        {
            //TODO: Implement token refresh logic
        }
    }
}
