using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class AuthRepo
    {

        public static async Task<ReturnModel<bool>> Register(string? username, string? code, string? password, string? confirmPassword)
        {
            if (DebugMode.SkipAuthReg) return new("R0", ReturnModelSeverityEnum.Ok, true);
            /* TODO: Register logic
             * Tu logika pro registraci
             *
             * Vrací standardní ReturnModel<bool> s kódem R0 při úspěchu, jinak s příslušným chybovým kódem a zprávou.
             * To be done
             */

            #region Validations out of Api
            if (string.IsNullOrWhiteSpace(username)) return new("R1", ReturnModelSeverityEnum.Error, false);
            if (string.IsNullOrWhiteSpace(password)) return new("R2", ReturnModelSeverityEnum.Error, false);
            if (string.IsNullOrWhiteSpace(confirmPassword)) return new("R3", ReturnModelSeverityEnum.Error, false);
            if (password != confirmPassword) return new("R4", ReturnModelSeverityEnum.Error, false);
            if (string.IsNullOrWhiteSpace(code)) return new("R5", ReturnModelSeverityEnum.Error, false);
            #endregion

            return new ("R0", ReturnModelSeverityEnum.Ok, true);
        }

        public static async Task<ReturnModel<bool>> Login(string? username, string? password, CancellationToken cancellationToken)
        {
            if (DebugMode.SkipAuthLogin) return new("A0", ReturnModelSeverityEnum.Ok, true);

            #region Validations out of Api
            if (string.IsNullOrWhiteSpace(username)) return new("A10", ReturnModelSeverityEnum.Error, false);
            if (string.IsNullOrWhiteSpace(password)) return new("A11", ReturnModelSeverityEnum.Error, false);
            #endregion

            var payload = new
            {
                username,
                password
            };

            // Použijeme tvůj worker. Pro login nastavíme requireAuth: false
            string? body = await HttpWorker.HttpPost("/auth/login", payload, requireAuth: false, cancellationToken);

            // Pokud worker vrátí null, znamená to, že v catch bloku došlo k chybě (výjimka, špatné URL apod.)
            if (body == null)
            {
                return new ReturnModel<bool>("A9", ReturnModelSeverityEnum.Error, false)
                {
                    DetailMessage = "API request failed or returned null."
                };
            }

            try
            {
                var rpcResponse = JsonSerializer.Deserialize<ReturnModel<JwtAuthResponse>>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (rpcResponse?.Body == null)
                {
                    return new ReturnModel<bool>("A9", ReturnModelSeverityEnum.Error, false) { DetailMessage = "Invalid API response structure." };
                }

                // Uložení tokenů
                Credentials.JwtAccessToken = rpcResponse.Body.AccessToken;
                Credentials.AccessTokenExpiresAtUtc = rpcResponse.Body.ExpiresAtUtc;
                Credentials.RefreshToken = rpcResponse.Body.RefreshToken;
                Credentials.RefreshTokenExpiresAtUtc = rpcResponse.Body.RefreshTokenExpiresAtUtc;
                Credentials.Role = (Role)int.Parse(rpcResponse.Body.Role);

                return new ReturnModel<bool>("A0", ReturnModelSeverityEnum.Ok, true);
            }
            catch (JsonException ex)
            {
                AppLogger.Error("Failed to deserialize login response", ex);
                return new ReturnModel<bool>("A9", ReturnModelSeverityEnum.Error, false) { DetailMessage = "Serialization error." };
            }
        }
    }
}
