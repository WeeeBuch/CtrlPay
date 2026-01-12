using Microsoft.AspNetCore.Mvc;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        [HttpPost]
        [Route("index")]
        public IActionResult Index(string message)
        {
            return Json(new { Id = 1, Jmeno = "Petr", Message = message });
        }
    }
}
