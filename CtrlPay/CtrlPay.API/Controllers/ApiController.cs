using CtrlPay.Core;
using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Transactions;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ApiController : Controller
    {
        //TODO: Metody API: 3
        /*
         * 
         * getaccount
         * getpayments by loyalcustomer
         * gettransactions by loyalcustomer
        */

        private readonly CtrlPayDbContext _db;

        public ApiController()
        {
            _db = new CtrlPayDbContext();
        }

        [HttpPost]
        [Route("index")]
        public IActionResult Index(string message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { Id = userId, Jmeno = userName, Message = message });
        }
        [HttpGet]
        [Route("get_transactions")]
        public IActionResult GetTransactions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            int accountIndex = user.LoyalCustomer.Account.Index;
            List<Entities.Transaction> transactions = _db.Transactions
                .Where(t => t.Account.Index == accountIndex)
                .ToList();
            List<TransactionApiDTO> transactionsDTO = new List<TransactionApiDTO>();
            foreach(var transaction in transactions)
            {
                transactionsDTO.Add(new TransactionApiDTO(transaction));
            }
            return Ok(transactionsDTO);
        }
    }
}
