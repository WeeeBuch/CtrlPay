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
            if (DebugMode.IsDebugMode) return new("R0", ReturnModelSeverityEnum.Ok, true);
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
            if (DebugMode.IsDebugMode) return new ("A0", ReturnModelSeverityEnum.Ok, true);

            #region Validations out of Api
            if (string.IsNullOrWhiteSpace(username)) return new("A10", ReturnModelSeverityEnum.Error, false);
            if (string.IsNullOrWhiteSpace(password)) return new("A11", ReturnModelSeverityEnum.Error, false);
            #endregion

            HttpClient httpClient = new();
            var payload = new
            {
                username,
                password
            };

            string payloadJson = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                payloadJson,
                Encoding.UTF8,
                "application/json"
            );
            
            string uri = Credentials.BaseUri + "/auth/login";

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(
                    uri,
                    content,
                    cancellationToken
                );
            }
            catch (Exception)
            {
                return new("A0", ReturnModelSeverityEnum.Error, false);
            }
            
            

            string body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ReturnModel<bool>("A9", ReturnModelSeverityEnum.Error, false) { DetailMessage = $"HTTP error {response.StatusCode}: {body}" };
            }

            var rpcResponse = JsonSerializer.Deserialize<ReturnModel<JwtAuthResponse>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Credentials.JwtAccessToken = rpcResponse.Body.AccessToken;
            Credentials.AccessTokenExpiresAtUtc = rpcResponse.Body.ExpiresAtUtc;
            Credentials.RefreshToken = rpcResponse.Body.RefreshToken;
            Credentials.RefreshTokenExpiresAtUtc = rpcResponse.Body.RefreshTokenExpiresAtUtc;

            return new ReturnModel<bool>("A0", ReturnModelSeverityEnum.Ok, true);
        }
    }
}
