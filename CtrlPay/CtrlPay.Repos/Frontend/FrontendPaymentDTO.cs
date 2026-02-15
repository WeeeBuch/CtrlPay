using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public class FrontendPaymentDTO
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public decimal ExpectedAmountXMR { get; set; }
    public decimal PaidAmountXMR { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? DueDate { get; set; }
}
