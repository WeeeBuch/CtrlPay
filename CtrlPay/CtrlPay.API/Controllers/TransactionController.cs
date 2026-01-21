using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        //TODO: Předělat na ReturnModel vracení
        private readonly CtrlPayDbContext _db;
        public TransactionController(CtrlPayDbContext ctrlPayDbContext)
        {
            _db = ctrlPayDbContext;
        }
        [HttpGet]
        [Route("my")]
        // GET : api/transactions/my
        public IActionResult My()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int accountIndex = user.LoyalCustomer.Account.Index;
            List<Entities.Transaction> transactions = _db.Transactions
                .Where(t => t.Account.Index == accountIndex)
                .ToList();
            List<TransactionApiDTO> transactionsDTO = new List<TransactionApiDTO>();
            foreach (var transaction in transactions)
            {
                transactionsDTO.Add(new TransactionApiDTO(transaction));
            }
            return Ok(transactionsDTO);
        }
        [HttpGet]
        [Route("credit")]
        // GET : api/transactions/credit
        public IActionResult Credit()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int? accountIndex = user.LoyalCustomer.Account.Index;
            if (accountIndex is null)
            {
                return BadRequest("User has no account.");
            }
            decimal credit = _db.Transactions
                .Where(t => t.Account.Index == accountIndex)
                .Where(t => t.Status == TransactionStatusEnum.Completed || t.Status == TransactionStatusEnum.Confirmed)
                .Sum(p => p.Amount);
            decimal ourXmr = _db.LoyalCustomers
                .Where(lc => lc.Account.Index == accountIndex)
                .First()
                .OurXMR;
            credit -= ourXmr;
            return Ok(credit);
        }
    }
}
