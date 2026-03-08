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
            if (result.Severity != ReturnModelSeverityEnum.Ok)
            {
                return Unauthorized(result);
            }

            // najdeme userId a roli pro token
            var user = _db.Users.First(u => u.Username == username);

            if (user.TwoFactorEnabled)
            {
                // vrátíme demo JWT pro TOTP
                JwtAuthResponse demoToken = _tokenService.GenerateAccessToken(user, isTotp: true);
                return Ok(new ReturnModel<JwtAuthResponse>("A5", ReturnModelSeverityEnum.Ok, demoToken));
            }

            // normální full JWT
            JwtAuthResponse fullToken = _tokenService.GenerateAccessToken(user, isTotp: false);
            return Ok(new ReturnModel<JwtAuthResponse>("A0", ReturnModelSeverityEnum.Ok, fullToken));
        }

        [HttpPost("verify-totp")]
        public IActionResult VerifyTotp(string totpToken, string totpCode)
        {
            // nejdřív extrahujeme principal z totp tokenu
            var principal = _tokenService.GetPrincipalFromToken(totpToken, ignoreExpiration: true);
            if (principal == null)
                return Unauthorized(new ReturnModel("A7", ReturnModelSeverityEnum.Error));

            // kontrola zda je to opravdu TOTP token
            var isTotpClaim = principal.FindFirst("IsTotp")?.Value;
            if (isTotpClaim != "true")
                return BadRequest(new ReturnModel("A8", ReturnModelSeverityEnum.Error));

            int userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            CtrlPayDbContext db = new CtrlPayDbContext();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return Unauthorized(new ReturnModel("A3", ReturnModelSeverityEnum.Error));

            ReturnModel authResponse = AuthLogic.TotpLogin(user.TwoFactorSecret, totpCode);
            bool validTotp = authResponse.Severity == ReturnModelSeverityEnum.Ok;
            if (!validTotp)
                return Unauthorized(authResponse);

            // vydáme plný access token
            var token = _tokenService.GenerateAccessToken(user, isTotp: false);
            return Ok(new ReturnModel<JwtAuthResponse>("A0", ReturnModelSeverityEnum.Ok, token));
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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            List<string> regCodes = _db.LoyalCustomers.Select(l => l.RegCode).ToList();
            if(!regCodes.Contains(request.Code))
            {
                return BadRequest(new ReturnModel("R6", ReturnModelSeverityEnum.Error));
            }

            var result = AuthLogic.AddUser(request.Username, request.Password, Role.Customer);
            if (result.Severity != ReturnModelSeverityEnum.Ok)
                return BadRequest(result);
            User newUser = result.Body;
            LoyalCustomer loyalCustomer = _db.LoyalCustomers.Where(l => l.RegCode == request.Code).FirstOrDefault();
            User user = _db.Users.Where(u => u.Id == newUser.Id).FirstOrDefault();
            user.LoyalCustomer = loyalCustomer;
            loyalCustomer.RegCode = null;
            await _db.SaveChangesAsync();
            return Ok(new ReturnModel("R0", ReturnModelSeverityEnum.Ok));
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Code { get; set; }
        }
    }
}

