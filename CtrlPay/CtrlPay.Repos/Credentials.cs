using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class Credentials
    {
        public static string JwtAccessToken { get; set; }
        // Default base URL – can be overridden from settings / API dialog
        public static string BaseUri { get; set; } = "https://www.action-games.cz/";
        public static DateTime AccessTokenExpiresAtUtc { get; set; }
        public static string RefreshToken { get; set; }
        public static DateTime RefreshTokenExpiresAtUtc { get; set; }
        public static Role Role { get; set; }


        public static void Clear()
        {
            AppLogger.Info($"Clearing credentials...");
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
