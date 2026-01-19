using CtrlPay.Core;
using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly CtrlPayDbContext _db = new CtrlPayDbContext();

        public AuthController(TokenService tokenService)
        {
            _db = new CtrlPayDbContext();
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        //POST : auth/login
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string username = request.Username;
            string password = request.Password;
            var result = AuthLogic.Login(username, password);
            if (result.ReturnCode != 0 && result.ReturnCode != 5)
            {
                return Unauthorized(result.Message);
            }

            // najdeme userId a roli pro token
            var user = _db.Users.First(u => u.Username == username);

            if (user.TwoFactorEnabled)
            {
                // vrátíme demo JWT pro TOTP
                var demoToken = _tokenService.GenerateAccessToken(user, isTotp: true);
                return Ok(demoToken);
            }

            // normální full JWT
            var token = _tokenService.GenerateAccessToken(user, isTotp: false);
            return Ok(token);
        }

        [HttpPost("verify-totp")]
        public IActionResult VerifyTotp(string totpToken, string totpCode)
        {
            // nejdřív extrahujeme principal z totp tokenu
            var principal = _tokenService.GetPrincipalFromToken(totpToken, ignoreExpiration: true);
            if (principal == null)
                return Unauthorized("Invalid or expired totp token.");

            // kontrola zda je to opravdu TOTP token
            var isTotpClaim = principal.FindFirst("IsTotp")?.Value;
            if (isTotpClaim != "true")
                return BadRequest("Token is not a TOTP token.");

            int userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            CtrlPayDbContext db = new CtrlPayDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Unauthorized("User not found.");

            AuthLogicReturnModel authResponse = AuthLogic.TotpLogin(user.TwoFactorSecret, totpCode);
            bool validTotp = authResponse.ReturnCode == 0;
            if (!validTotp)
                return Unauthorized(authResponse.Message);

            // vydáme plný access token
            var token = _tokenService.GenerateAccessToken(user, isTotp: false);
            return Ok(token);
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(string accessToken, string refreshToken)
        {
            // request.RefreshToken a request.AccessToken (starý JWT)
            var principal = _tokenService.GetPrincipalFromToken(accessToken, ignoreExpiration: true);
            if (principal == null)
                return Unauthorized();

            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            User user = _db.Users.FirstOrDefault(u => u.Id == userId);

            var storedRefresh = _db.RefreshTokens
                .FirstOrDefault(r => r.Token == refreshToken && r.User.Id == userId);

            if (storedRefresh == null || !storedRefresh.IsActive)
                return Unauthorized();

            JwtAuthResponse? newJwt = _tokenService.GenerateAccessToken(user, isTotp: false);
            var newRefresh = _tokenService.GenerateRefreshToken();

            // aktualizace DB
            storedRefresh.Token = newRefresh.Token;
            storedRefresh.ExpiresAtUtc = newRefresh.Expires;
            _db.SaveChanges();

            return Ok(newJwt);
        }
        [HttpPost("register")]
        public IActionResult Register(string username, string password, string roleName)
        {
            // najdeme roli
            var role = _db.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
                return BadRequest("Invalid role.");
            var result = AuthLogic.AddUser(username, password, role);
            if (result.ReturnCode != 0)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}

