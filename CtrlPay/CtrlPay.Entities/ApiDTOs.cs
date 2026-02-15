using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class TransactionApiDTO
    {
        public int Id { get; set; }
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        public string TransactionIdXMR { get; set; }
        public TransactionTypeEnum Type { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PaymentId { get; set; }

        public TransactionApiDTO()
        {
            
        }
        public TransactionApiDTO(int id, int addressId, int accountId, string transactionIdXMR, TransactionTypeEnum type, TransactionStatusEnum status, decimal amount, decimal fee, DateTime timestamp, int? paymentId)
        {
            Id = id;
            AddressId = addressId;
            AccountId = accountId;
            TransactionIdXMR = transactionIdXMR;
            Type = type;
            Status = status;
            Amount = amount;
            Fee = fee;
            Timestamp = timestamp;
            PaymentId = paymentId;
        }

        public TransactionApiDTO(Transaction transaction)
        {
            Id = transaction.Id;
            AddressId = transaction.Address.Id;
            AccountId = transaction.Account.Index;
            TransactionIdXMR = transaction.TransactionIdXMR;
            Type = transaction.Type;
            Status = transaction.Status;
            Amount = transaction.Amount;
            Fee = transaction.Fee;
            Timestamp = transaction.Timestamp;
            if (transaction.Payment != null)
            {
                PaymentId = transaction.Payment.Id;
            }
        }
    }
    public class PaymentApiDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public int? AddressId { get; set; }
        public decimal ExpectedAmountXMR { get; set; }
        public decimal PaidAmountXMR { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Title { get; set; }

        public PaymentApiDTO()
        {
            
        }
        public PaymentApiDTO(Payment payment)
        {
            Id = payment.Id;
            CustomerId = payment.CustomerId;
            AccountId = payment.AccountId;
            AddressId = payment.AddressId;
            ExpectedAmountXMR = payment.ExpectedAmountXMR;
            PaidAmountXMR = payment.PaidAmountXMR;
            Status = payment.Status;
            CreatedAt = payment.CreatedAt;
            PaidAt = payment.PaidAt ?? default;
            DueDate = payment.DueDate ?? default;
            Title = payment.Title;
        }
    }

    public class CustomerApiDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Physical { get; set; }
        public string? Company { get; set; }
        public bool? IsLoyal { get; set; }
        public string? Username { get; set; }
        public int? AccountID { get; set; }
        public string? BaseAddress { get; set; }
        public CustomerApiDTO()
        {
            
        }
        public CustomerApiDTO(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Title = customer.Title;
            Address = customer.Address;
            City = customer.City;
            PostalCode = customer.PostalCode;
            Email = customer.Email;
            Phone = customer.Phone;
            Physical = customer.Physical;
            Company = customer.Company;
            IsLoyal = false;
        }
        public CustomerApiDTO(Customer customer, LoyalCustomer loyalCustomer, User user)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Title = customer.Title;
            Address = customer.Address;
            City = customer.City;
            PostalCode = customer.PostalCode;
            Email = customer.Email;
            Phone = customer.Phone;
            Physical = customer.Physical;
            Company = customer.Company;
            IsLoyal = true;
            Username = user.Username;
            AccountID = loyalCustomer.Account.Index;
            BaseAddress = loyalCustomer.Account.BaseAddress.AddressXMR;
        }
    }
}
