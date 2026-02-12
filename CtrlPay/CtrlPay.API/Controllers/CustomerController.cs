using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Security.Claims;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CtrlPayDbContext _db;
        private readonly bool _mergedAccountants;


        public CustomerController(IConfiguration configuration)
        {
            _db = new CtrlPayDbContext();
            _mergedAccountants = configuration.GetValue<bool>("MergeAccountantAndPayrollAccountant");
        }
        [HttpPost("api/customers/create")]
        // POST : api/customers/create
        public IActionResult CreateCustomer([FromBody] CustomerApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant || role != Role.Admin)
            {
                return Forbid();
            }
            Customer customer = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Title = request.Title,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                Email = request.Email,
                Phone = request.Phone,
                Physical = request.Physical,
                Company = request.Company
            };
            _db.Customers.Add(customer);
            _db.SaveChanges();
            return Ok(new ReturnModel("Z0", ReturnModelSeverityEnum.Ok));
        }
        [HttpGet("api/customers/all")]
        // GET : api/customers/all
        public IActionResult GetCustomers()
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant || role != Role.Admin)
            {
                return Forbid();
            }
            List<CustomerApiDTO> customers = new();
            _db.Customers.ToList().ForEach(c =>
            {
                customers.Add(c.LoyalCustomer == null ? new CustomerApiDTO(c) : new CustomerApiDTO(c, c.LoyalCustomer, c.LoyalCustomer.Users.First()));
            });
            return Ok(customers);
        }

        [HttpPost("api/customers/edit")]
        // POST : api/customers/edit
        public IActionResult EditCustomer([FromBody] CustomerApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant || role != Role.Admin)
            {
                return Forbid();
            }
            Customer customer = _db.Customers.Where(c => c.Id == request.Id).First();
            if (customer == null)
            {
                return NotFound(new ReturnModel("Z1", ReturnModelSeverityEnum.Error));
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Title = request.Title;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.PostalCode = request.PostalCode;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Physical = request.Physical;
            customer.Company = request.Company;

            _db.Update(customer);
            _db.SaveChanges();

            return Ok(new ReturnModel("Z0", ReturnModelSeverityEnum.Ok));
        }
        [HttpDelete("api/customers/delete/{id}")]
        // DELETE : api/customers/delete/{id}
        public IActionResult DeleteCustomer(int id)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant || role != Role.Admin)
            {
                return Forbid();
            }
            Customer customer = _db.Customers.Where(c => c.Id == id).First();
            if (customer == null)
            {
                return NotFound(new ReturnModel("Z1", ReturnModelSeverityEnum.Error));
            }
            _db.Remove(customer);
            _db.SaveChanges();
            return Ok(new ReturnModel("Z0", ReturnModelSeverityEnum.Ok));
        }
        [HttpPost]
        [Route("api/customers/promote/{id}")]
        // POST : api/customers/promote/{id}
        public IActionResult PromoteToLoyalCustomer(int id)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant || role != Role.Admin)
            {
                return Forbid();
            }
            //TODO: Dodělat promote to loyal customer
            return Ok(new ReturnModel("Z0", ReturnModelSeverityEnum.Ok));
        }
    }
}
