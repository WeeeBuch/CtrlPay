using CtrlPay.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        //TODO: Metody API:
        /*
         * login - jwt věci
         * getaccount
         * getpayments by loyalcustomer
         * gettransactions by loyalcustomer
        */

        [HttpPost]
        [Route("index")]
        public IActionResult Index(string message)
        {
            return Json(new { Id = 1, Jmeno = "Petr", Message = message });
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string username, string password)
        {
            AuthLogic.Login(username, password);
            return Json(new { Success = true });
        }
    }
}
