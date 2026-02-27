using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class RegistrationEmailModel
    {
        public string CustomerName { get; set; } = string.Empty;

        public string RegistrationCode { get; set; } = string.Empty;

        public string DownloadLink { get; set; } = string.Empty;

        public string SupportEmail { get; set; } = string.Empty;

        public RegistrationEmailModel(string customerName, string registrationCode, string downloadLink, string supportEmail)
        {
            CustomerName = customerName;
            RegistrationCode = registrationCode;
            DownloadLink = downloadLink;
            SupportEmail = supportEmail;
        }
    }
    public class OverpaidEmailModel
    {
        public string PaymentId { get; set; } = string.Empty;
        public decimal OverpaidAmountXMR => PaidAmountXMR - ExpectedAmountXMR;
        public decimal ExpectedAmountXMR { get; set; }
        public decimal PaidAmountXMR { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;

    }
    public class PaymentReminderEmailModel
    {
        public string CustomerName { get; set; }
        public string PaymentId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public decimal ExpectedAmountXMR { get; set; }
        public decimal PaidAmountXMR { get; set; }
        public decimal RemainingAmountXMR => ExpectedAmountXMR - PaidAmountXMR;
        public PaymentReminderEmailModel()
        {
            
        }
    }
}
