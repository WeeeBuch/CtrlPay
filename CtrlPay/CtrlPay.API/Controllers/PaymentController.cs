using CtrlPay.API.BackgroundServices;
using CtrlPay.Core;
using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Security.Claims;
using System.Text.Json;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        //TODO: Předělat na ReturnModel vracení
        private readonly CtrlPayDbContext _db;
        private readonly MoneroRpcOptions _rpcOptions;
        private readonly bool _mergedAccountants;
        public PaymentController(IOptions<MoneroRpcOptions> rpcOptions, IConfiguration configuration)
        {
            _db = new CtrlPayDbContext();
            _rpcOptions = rpcOptions.Value;
            _mergedAccountants = configuration.GetValue<bool>("MergeAccountantAndPayrollAccountant");
        }
        [HttpGet]
        [Route("my")]
        // GET : api/payments/my
        public IActionResult My()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int? accountIndex = user.LoyalCustomer.Account.Index;
            if (accountIndex == null)
            {
                return Forbid(JsonSerializer.Serialize(new ReturnModel("P1", ReturnModelSeverityEnum.Error)));
            }
            List<Entities.Payment> payments = _db.Payments
                .Where(p => p.Account.Index == accountIndex)
                .ToList();

            List<PaymentApiDTO> paymentsDTO = new List<PaymentApiDTO>();
            foreach (var payment in payments)
            {
                paymentsDTO.Add(new PaymentApiDTO(payment));
            }
            return Ok(new ReturnModel<List<PaymentApiDTO>>("P0", ReturnModelSeverityEnum.Ok, paymentsDTO));
        }
        [HttpGet]
        [Route("amount-due")]
        // GET : api/payments/amount-due
        public IActionResult AmountDue()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int accountIndex = user.LoyalCustomer.Account.Index;
            List<Entities.Payment> debts = _db.Payments
                .Where(p => p.Account.Index == accountIndex)
                .Where(p => p.Status == PaymentStatusEnum.PartiallyPaid || p.Status == PaymentStatusEnum.WaitingForPayment)
                .ToList();
            decimal sum = debts.Sum(d => d.ExpectedAmountXMR - d.PaidAmountXMR);
            return Ok(new ReturnModel<decimal>("P0", ReturnModelSeverityEnum.Ok, sum));
        }
        [HttpPost]
        [Route("pay-from-credit")]
        // POST : api/payments/pay-from-credit
        public async Task<IActionResult> PayFromCredit([FromBody] PayFromCreditRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            Payment payment = _db.Payments.Where(p => p.Id == request.PaymentId).First();
            if (user == null)
            {
                return Forbid(JsonSerializer.Serialize(new ReturnModel("A3", ReturnModelSeverityEnum.Error)));
            }
            if (user.LoyalCustomer == null)
            {
                return Forbid(JsonSerializer.Serialize(new ReturnModel("A6", ReturnModelSeverityEnum.Error)));
            }
            if (payment == null)
            {
                return NotFound(new ReturnModel("P2", ReturnModelSeverityEnum.Error));
            }
            if (payment.Customer != user.LoyalCustomer.Customer)
            {
                //neni jeho
                return Forbid(JsonSerializer.Serialize(new ReturnModel("P3", ReturnModelSeverityEnum.Error)));
            }
            if (payment.Status == PaymentStatusEnum.Paid)
            {
                return BadRequest(new ReturnModel("P4", ReturnModelSeverityEnum.Warning));
            }

            await PaymentProcessing.PayFromCredit(user.LoyalCustomer, payment, new CancellationToken());
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
    }

    public class CreatePaymentRequest
    {
    }

    public class PayFromCreditRequest
    {
        public int PaymentId { get; set; }
    }
}
