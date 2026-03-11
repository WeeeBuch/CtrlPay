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
        //GET: api/admin/users
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
            //TODO: Implement user creation logic
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpPost]
        [Route("users/update")]
        //POST: api/admin/users/update
        public IActionResult UpdateUser([FromBody] UserApiDTO user)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value!);
            if (role != Role.Admin)
            {
                return Forbid();
            }
            //TODO: Implement user update logic
            return Ok(new ReturnModel("G0", ReturnModelSeverityEnum.Ok));
        }

        [HttpDelete]
        [Route("users/delete/{id}")]
        //DELETE: api/admin/users/delete/{id}
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
