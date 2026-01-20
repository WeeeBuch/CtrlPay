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
    }
}
