using Castle.Core.Resource;
using CtrlPay.API.BackgroundServices;
using CtrlPay.Core;
using CtrlPay.DB;
using CtrlPay.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Prng;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace CtrlPay.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly CtrlPayDbContext _db;
        private readonly EmailService _post;
        private readonly MailRenderService _mailRenderer;
        public PaymentController(EmailService post, MailRenderService mailRenderer)
        {
            _db = new CtrlPayDbContext();
            _post = post;
            _mailRenderer = mailRenderer;
        }
        [HttpGet]
        [Route("my")]
        // GET : api/payments/my
        public IActionResult My()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            if (user == null)
            {
                return Forbid();
            }
            if (user.LoyalCustomer == null)
            {
                return Forbid();
            }
            int? accountIndex = user.LoyalCustomer.Account.Index;
            if (accountIndex == null)
            {
                return Forbid();
            }
            List<Entities.Payment> payments = _db.Payments
                .Where(p => p.Account.Index == accountIndex)
                .ToList();

            List<PaymentApiDTO> paymentsDTO = new List<PaymentApiDTO>();
            foreach (var payment in payments)
            {
                paymentsDTO.Add(new PaymentApiDTO(payment));
            }
            return Ok(new ReturnModel<List<PaymentApiDTO>>("P0", ReturnModelSeverityEnum.Ok, paymentsDTO));
        }
        [HttpGet]
        [Route("amount-due")]
        // GET : api/payments/amount-due
        public IActionResult AmountDue()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            if (user == null)
            {
                return Forbid();
            }
            if (user.LoyalCustomer == null)
            {
                return Forbid();
            }
            int accountIndex = user.LoyalCustomer.Account.Index;
            List<Entities.Payment> debts = _db.Payments
                .Where(p => p.Account.Index == accountIndex)
                .Where(p => p.Status == PaymentStatusEnum.PartiallyPaid || p.Status == PaymentStatusEnum.WaitingForPayment || p.Status == PaymentStatusEnum.Unpaid)
                .ToList();
            decimal sum = debts.Sum(d => d.ExpectedAmountXMR - d.PaidAmountXMR);
            return Ok(new ReturnModel<decimal>("P0", ReturnModelSeverityEnum.Ok, sum));
        }
        [HttpPost]
        [Route("pay-from-credit")]
        // POST : api/payments/pay-from-credit
        public async Task<IActionResult> PayFromCredit([FromBody] PayFromCreditRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user = _db.Users.Where(u => u.Id.ToString() == userId).First();
            Payment payment = _db.Payments.Where(p => p.Id == request.PaymentId).First();
            if (user == null)
            {
                return Forbid();
            }
            if (user.LoyalCustomer == null)
            {
                return Forbid();
            }
            if (payment == null)
            {
                return NotFound(new ReturnModel("P2", ReturnModelSeverityEnum.Error));
            }
            if (payment.Customer != user.LoyalCustomer.Customer)
            {
                //neni jeho
                return Forbid();
            }
            if (payment.Status == PaymentStatusEnum.Paid)
            {
                return BadRequest(new ReturnModel("P4", ReturnModelSeverityEnum.Warning));
            }

            await PaymentProcessing.PayFromCredit(user.LoyalCustomer, payment, new CancellationToken());
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
        [HttpGet]
        [Route("all")]
        // GET : api/payments/all
        public IActionResult All()
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            List<Entities.Payment> payments = _db.Payments.ToList();
            List<PaymentApiDTO> paymentsDTO = new List<PaymentApiDTO>();
            foreach (var payment in payments)
            {
                paymentsDTO.Add(new PaymentApiDTO(payment));
            }
            return Ok(new ReturnModel<List<PaymentApiDTO>>("P0", ReturnModelSeverityEnum.Ok, paymentsDTO));
        }
        [HttpPost]
        [Route("create")]
        // POST : api/payments/create
        public IActionResult Create([FromBody] PaymentApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            Customer customer = _db.Customers.Where(c => c.Id == request.CustomerId).First();
            if (customer == null)
            {
                return NotFound(new ReturnModel("C2", ReturnModelSeverityEnum.Error));
            }
            Payment payment = new Payment()
            {
                Customer = _db.Customers.Where(c => c.Id == request.CustomerId).First(),
                Account = customer.LoyalCustomer.Account,
                ExpectedAmountXMR = request.ExpectedAmountXMR,
                PaidAmountXMR = request.PaidAmountXMR,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                DueDate = request.DueDate,
                Title = request.Title
            };
            _db.Payments.Add(payment);
            _db.SaveChanges();
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
        [HttpPost]
        [Route("update")]
        // POST : api/payments/update
        public IActionResult Update([FromBody] PaymentApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            Payment payment = _db.Payments.Where(p => p.Id == request.Id).First();
            if (payment == null)
            {
                return NotFound(new ReturnModel("P2", ReturnModelSeverityEnum.Error));
            }
            payment.Id = request.Id;
            payment.CustomerId = request.CustomerId;
            payment.AccountId = payment.Customer.LoyalCustomer.Account.Index;
            payment.ExpectedAmountXMR = request.ExpectedAmountXMR;
            payment.PaidAmountXMR = request.PaidAmountXMR;
            payment.Status = request.Status;
            payment.CreatedAt = request.CreatedAt;
            payment.Title = request.Title;
            if (request.PaidAt != DateTimeOffset.MinValue)
            {
                payment.PaidAt = request.PaidAt;
            }  
            payment.DueDate = request.DueDate;
            _db.SaveChanges();
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
        [HttpDelete]
        [Route("delete/{id}")]
        // DELETE : api/payments/delete/{id}
        public IActionResult Delete(int id)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            Payment payment = _db.Payments.Where(p => p.Id == id).First();
            if (payment == null)
            {
                return NotFound(new ReturnModel("P2", ReturnModelSeverityEnum.Error));
            }
            if (payment.Status == PaymentStatusEnum.PartiallyPaid ||payment.Status == PaymentStatusEnum.Paid ||payment.Status == PaymentStatusEnum.Overpaid)
            {
                return BadRequest(new ReturnModel("P6", ReturnModelSeverityEnum.Warning));
            }
            
            _db.Payments.Remove(payment);
            _db.SaveChanges();
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
        [HttpGet]
        [Route("overpaid")]
        // GET : api/payments/overpaid
        public IActionResult Overpaid()
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            List<Entities.Payment> payments = _db.Payments
                .Where(p => p.Status == PaymentStatusEnum.Overpaid)
                .ToList();
            List<PaymentApiDTO> paymentsDTO = new List<PaymentApiDTO>();
            foreach (var payment in payments)
            {
                paymentsDTO.Add(new PaymentApiDTO(payment));
            }
            return Ok(new ReturnModel<List<PaymentApiDTO>>("P0", ReturnModelSeverityEnum.Ok, paymentsDTO));
        }
        [HttpPost]
        [Route("overpay-to-credit")]
        // POST : api/payments/overpay-to-credit
        public IActionResult OverpaidToCredit([FromBody] PaymentApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
             Payment payment = _db.Payments.Where(p => p.Id == request.Id).First();
            if (payment == null)
            {
                return NotFound(new ReturnModel("P2", ReturnModelSeverityEnum.Error));
            }
            if (payment.Status != PaymentStatusEnum.Overpaid)
            {
                return BadRequest(new ReturnModel("P5", ReturnModelSeverityEnum.Error));
            }
            decimal surplus = payment.PaidAmountXMR - payment.ExpectedAmountXMR;
            if (surplus <= 0)
            {
                return BadRequest(new ReturnModel("P5", ReturnModelSeverityEnum.Error));
            }
            LoyalCustomer loyalCustomer = _db.LoyalCustomers.Where(lc => lc.CustomerId == payment.CustomerId).First();
            Transaction newInternalTransaction = new Transaction()
            {
                Account = loyalCustomer.Account,
                Address = loyalCustomer.Account.BaseAddress,
                TransactionIdXMR = $"internal-{Convert.ToBase64String(RandomNumberGenerator.GetBytes(4))}",
                Amount = surplus,
                Fee = 0,
                Status = TransactionStatusEnum.Completed,
                Type = TransactionTypeEnum.Internal,
                Timestamp = DateTime.UtcNow
            };
            _db.Transactions.Add(newInternalTransaction);
            payment.PaidAmountXMR -= surplus;
            payment.Status = PaymentStatusEnum.Paid;
            _db.SaveChanges();
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));
        }
        [HttpPost]
        [Route("send-reminder")]
        // POST: api/payments/send-reminder
        public async Task<IActionResult> SendReminder([FromBody] PaymentApiDTO request)
        {
            Role role = (Role)int.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
            if (role != Role.Accountant && role != Role.Admin)
            {
                return Forbid();
            }
            Payment payment = _db.Payments.Where(p => p.Id == request.Id).FirstOrDefault();
            PaymentReminderEmailModel model = new PaymentReminderEmailModel()
            {
                CustomerName = payment.Customer.FullName,
                PaymentId = payment.Id.ToString(),
                CreatedAt = payment.CreatedAt,
                DueDate = payment.DueDate,
                ExpectedAmountXMR = payment.ExpectedAmountXMR,
                PaidAmountXMR = payment.PaidAmountXMR,
            };
            string mail = await _mailRenderer.RenderToStringAsync("Mails/Reminder", model);
            _post.SendAsync(payment.Customer.Email, "Upominka", mail);
            return Ok(new ReturnModel("P0", ReturnModelSeverityEnum.Ok));

        }
    }
    public class CreatePaymentRequest
    {
        public int CustomerId { get; set; }
        public decimal ExpectedAmountXMR { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class PayFromCreditRequest
    {
        public int PaymentId { get; set; }
    }
}
