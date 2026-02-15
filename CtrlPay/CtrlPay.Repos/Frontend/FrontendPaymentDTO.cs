using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
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
    public StatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Title { get; set; }

    public FrontendPaymentDTO()
    {
        
    }
    public FrontendPaymentDTO(PaymentApiDTO dto)
    {
        Id = dto.Id;
        CustomerId = dto.CustomerId;
        ExpectedAmountXMR = dto.ExpectedAmountXMR;
        PaidAmountXMR = dto.PaidAmountXMR;
        Status = StatusConverter.ConvertPaymentStatusToFrontendStatus(dto.Status);
        CreatedAt = dto.CreatedAt;
        PaidAt = dto.PaidAt ?? default;
        DueDate = dto.DueDate ?? default;
        Title = dto.Title;
    }

    public PaymentApiDTO ToApiDto()
    {
        return new PaymentApiDTO
        {
            Id = Id,
            CustomerId = CustomerId,
            ExpectedAmountXMR = ExpectedAmountXMR,
            PaidAmountXMR = PaidAmountXMR,
            Status = StatusConverter.ConvertFrontendStatusToPaymentStatus(Status),
            CreatedAt = CreatedAt,
            PaidAt = PaidAt,
            DueDate = DueDate,
            Title = Title
        };
    }
}
