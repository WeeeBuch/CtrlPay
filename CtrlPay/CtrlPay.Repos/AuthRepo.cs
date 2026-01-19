using CtrlPay.Entities;
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
        public static string ErrorMessage = "";

        public static bool Register(string username, string code, string password, string confirmPassword)
        {
            /* TODO: Register logic
             * Tu logika pro registraci
             *
             * Vrací true, pokud je registrace úspěšná, jinak false
             */

            #region Validations out of Api

            if (username == null || username.Trim() == "")
            {
                ErrorMessage = "Uživatelské jméno nesmí být prázdné.";
                return false;
            }

            if (password == null || password.Trim() == "")
            {
                ErrorMessage = "Heslo nesmí být prázdné.";
                return false;
            }

            if (confirmPassword == null || confirmPassword.Trim() == "")
            {
                ErrorMessage = "Potvrzení hesla nesmí být prázdné.";
                return false;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Hesla se neshodují.";
                return false;
            }

            if (code == null || code.Trim() == "")
            {
                ErrorMessage = "Registrační kód nesmí být prázdný.";
                return false;
            }

            #endregion

            return true;
        }

        public static async Task<bool> Login(string username, string password, CancellationToken cancellationToken)
        {
            HttpClient httpClient = new HttpClient();
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
            
            string uri = Credentials.BaseUri + "/api/auth/login";

            
            HttpResponseMessage response = await httpClient.PostAsync(
                uri,
                content,
                cancellationToken
            );
            

            string body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"HTTP error {response.StatusCode}: {body}";
                return false;
            }

            var rpcResponse = JsonSerializer.Deserialize<JwtAuthResponse>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Credentials.JwtAccessToken = rpcResponse.AccessToken;
            Credentials.AccessTokenExpiresAtUtc = rpcResponse.ExpiresAtUtc;
            Credentials.RefreshToken = rpcResponse.RefreshToken;
            Credentials.RefreshTokenExpiresAtUtc = rpcResponse.RefreshTokenExpiresAtUtc;

            return true;
        }

        public static string RegisterFailedMessage() => ErrorMessage == "" ? "Něco je špatně. :)" : ErrorMessage;
        public static string LoginFailedMessage() => ErrorMessage == "" ? "Něco je špatně. :)" : ErrorMessage;
    }
}
