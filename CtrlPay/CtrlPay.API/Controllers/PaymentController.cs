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
    }
}
