using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : Controller
    {
        public CtrlPayDbContext _db { get; set; }
        public AdminController(CtrlPayDbContext ctrlPayDbContext)
        {
            _db = ctrlPayDbContext;
        }

        [HttpGet]
        [Route("users")]
        public IActionResult Users()
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }

            List<UserApiDTO> users = new List<UserApiDTO>();
            foreach (var user in _db.Users)
            {
                users.Add(new UserApiDTO(user));
            }

            return Ok(new ReturnModel<List<UserApiDTO>>("G0", ReturnModelSeverityEnum.Ok, users));
        }
    }
}
