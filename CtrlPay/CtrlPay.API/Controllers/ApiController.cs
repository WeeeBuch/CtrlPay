using CtrlPay.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

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

        [HttpPost]
        [Route("index")]
        public IActionResult Index(string message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { Id = userId, Jmeno = userName, Message = message });
        }

        
    }
}
