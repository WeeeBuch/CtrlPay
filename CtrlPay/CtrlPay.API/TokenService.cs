using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtrlPay.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace CtrlPay.API
{
    public class TokenService
    {
        private readonly IOptions<JwtOptions> _options;
        private readonly RsaSecurityKey _signingKey;
        private readonly SigningCredentials _signingCredentials;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options;
            var rsa = RSA.Create();
            rsa.ImportFromPem(_options.Value.PrivateKeyPem.ToCharArray());
            _signingKey = new RsaSecurityKey(rsa);
            _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
        }
        public JwtAuthResponse GenerateAccessToken(User user, bool isTotp = false)
        {
            var userId = user.Id;
            var username = user.Username;
            var role = user.Role;
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_options.Value.AccessTokenMinutes);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim("IsTotp", isTotp ? "true" : "false")
        };

            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            return new JwtAuthResponse
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = (int)(expires - now).TotalSeconds,
                ExpiresAtUtc = expires,
                UserId = userId,
                Username = username,
                Role = role.ToString(),
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.Expires,
                IsTotp = isTotp
            };
        }
        public JwtAuthResponse GenerateAccessToken(User user, string refreshToken, DateTime refreshTokenExpires, bool isTotp = false)
        {
            var userId = user.Id;
            var username = user.Username;
            var role = user.Role;
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_options.Value.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("IsTotp", isTotp ? "true" : "false")
            };

            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtAuthResponse
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = (int)(expires - now).TotalSeconds,
                ExpiresAtUtc = expires,
                UserId = userId,
                Username = username,
                Role = role.ToString(),
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpires,
                IsTotp = isTotp
            };
        }

        public (string Token, DateTime Expires) GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(randomBytes);
            var expires = DateTime.UtcNow.AddDays(_options.Value.RefreshTokenDays);
            return (token, expires);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token, bool ignoreExpiration = false)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Value.Issuer,
                ValidAudience = _options.Value.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = !ignoreExpiration
            };

            try
            {
                var principal = handler.ValidateToken(token, validationParams, out var securityToken);

                if (securityToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.Ordinal))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

}
