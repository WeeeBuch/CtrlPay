using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly CtrlPayDbContext _db;
        public PaymentController()
        {
            _db = new CtrlPayDbContext();
        }
        [HttpGet]
        [Route("my")]
        // GET : api/payments/my
        public IActionResult My()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int accountIndex = user.LoyalCustomer.Account.Index;
            List<Entities.Payment> payments = _db.Payments
                .Where(p => p.Account.Index == accountIndex)
                .ToList();

            List<PaymentApiDTO> paymentsDTO = new List<PaymentApiDTO>();
            foreach (var payment in payments)
            {
                paymentsDTO.Add(new PaymentApiDTO(payment));
            }
            return Ok(paymentsDTO);
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
            return Ok(sum);
        }

    }
}
