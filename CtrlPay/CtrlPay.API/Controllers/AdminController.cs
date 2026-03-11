using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [HttpPost]
        [Route("users/create")]
        public IActionResult CreateUser([FromBody] UserApiDTO user)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            User newUser = new User
            {
                Role = user.Role,
                LoyalCustomer = _db.LoyalCustomers.FirstOrDefault(l => l.Id == user.Id),
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                TwoFactorEnabled = user.TwoFactorEnabled
            };
            _db.Users.Add(newUser);
            _db.SaveChanges();
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpPost]
        [Route("users/update")]
        public IActionResult UpdateUser([FromBody] UserApiDTO user)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);
            if (dbUser == null)
            {
                return NotFound(new ReturnModel("G1", ReturnModelSeverityEnum.Error));
            }
            dbUser.Role = user.Role;
            dbUser.LoyalCustomer = _db.LoyalCustomers.FirstOrDefault(l => l.Id == user.Id);
            dbUser.Username = user.Username;
            dbUser.PasswordHash = user.PasswordHash;
            dbUser.PasswordSalt = user.PasswordSalt;
            dbUser.TwoFactorEnabled = user.TwoFactorEnabled;
            _db.SaveChanges();
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpPost]
        [Route("users/delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == id);
            if (dbUser == null)
            {
                return NotFound(new ReturnModel("G1", ReturnModelSeverityEnum.Error));
            }
            _db.Users.Remove(dbUser);
            _db.SaveChanges();
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));

        }
    }
}
