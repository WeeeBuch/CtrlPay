using CtrlPay.Core;
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
        private readonly CtrlPayDbContext _db;
        public AdminController(CtrlPayDbContext ctrlPayDbContext)
        {
            _db = ctrlPayDbContext;
        }

        [HttpGet]
        [Route("users")]
        //GET: api/admin/users
        public IActionResult Users()
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }

            List<UserApiDTO> users = new();
            foreach (var user in _db.Users.ToList())
            {
                users.Add(user.Role == Role.Customer ? new UserApiDTO(user, user.LoyalCustomer) : new UserApiDTO(user));
            }

            return Ok(new ReturnModel<List<UserApiDTO>>("G0", ReturnModelSeverityEnum.Ok, users));
        }

        [HttpPost]
        [Route("users/create")]
        //POST: api/admin/users/create
        public IActionResult CreateUser([FromBody] UserApiDTO user)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            User newUser = AuthLogic.AddUser(user.Username, user.Password, user.Role).Body;
            User dbUser = _db.Users.FirstOrDefault(u => u.Id == newUser.Id);
            dbUser.TwoFactorEnabled = user.TwoFactorEnabled;
            Customer cust = _db.Customers.FirstOrDefault(c => c.Id == user.CustomerId);
            if(user.Role == Role.Customer && cust.LoyalCustomer != null)
            {
                dbUser.LoyalCustomer = cust.LoyalCustomer;
            }
            _db.Users.Update(dbUser);
            _db.SaveChanges();

            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpPost]
        [Route("users/update")]
        //POST: api/admin/users/update
        public async Task<IActionResult> UpdateUser([FromBody] UserApiDTO user)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            User dbUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);
            if (dbUser == null) 
            {
                return NotFound(new ReturnModel("G1", ReturnModelSeverityEnum.Error));
            }
            dbUser.Username = user.Username;
            dbUser.Role = user.Role;
            dbUser.TwoFactorEnabled = user.TwoFactorEnabled;

            if(user.Password != "")
            {
                AuthLogic.ChangePassword(dbUser.Id, user.Password);
            }

            await _db.SaveChangesAsync();
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpDelete]
        [Route("users/delete/{id}")]
        //DELETE: api/admin/users/delete/{id}
        public async Task<IActionResult> DeleteUser(int id)
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
            await _db.SaveChangesAsync();
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));

        }
    }
}
