using CtrlPay.Core;
using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly CtrlPayDbContext _db;
        private readonly MoneroRpcOptions _rpcOptions;
        public AccountController(IOptions<MoneroRpcOptions> rpcOptions)
        {
            _db = new CtrlPayDbContext();
            _rpcOptions = rpcOptions.Value;
        }

        [HttpPost]
        [Route("one-time-address")]
        // POST : api/account/one-time-address
        public IActionResult OneTimeAddress([FromBody] OneTimeAddressRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = _db.Users.Where(u => u.Id.ToString() == userId).First();

            if (user == null)
            {
                return Unauthorized(new ReturnModel("A3", ReturnModelSeverityEnum.Error));
            }
            var payment = _db.Payments.Where(p => p.Id == request.PaymentId).First();
            string uri = $"http://{_rpcOptions.Host}:{_rpcOptions.Port}/json_rpc";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(_rpcOptions.Username, _rpcOptions.Password),
                PreAuthenticate = false, // u Digest MUSÍ být false
                UseProxy = false
            };

            var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            Address oneTimeAddress = XMRComs.GenerateOneTimeAddressForLoyalCustomer(user.LoyalCustomer, payment, httpClient, uri, new CancellationToken()).Result;
            return Created("", new ReturnModel<string>("C0", ReturnModelSeverityEnum.Ok, oneTimeAddress.AddressXMR));
        }

        [HttpGet]
        [Route("credit-address")]
        // GET : api/account/credit-address
        public IActionResult CreditAddress()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            if (user == null)
            {
                return Unauthorized(new ReturnModel("A3", ReturnModelSeverityEnum.Error));
            }
            var loyalCustomer = user.LoyalCustomer;
            if (loyalCustomer == null)
            {
                return BadRequest(new ReturnModel("C1", ReturnModelSeverityEnum.Error));
            }
            string creditAddress = loyalCustomer.Account.BaseAddress.AddressXMR;
            return Ok(new ReturnModel<string>("C1", ReturnModelSeverityEnum.Ok, creditAddress));
        }
    }

    public class OneTimeAddressRequest
    {
        public int PaymentId { get; set; }
    }
}
