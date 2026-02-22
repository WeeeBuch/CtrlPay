using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend;

public class FrontendCustomerDTO : IEditableObject
{
    public int Id {  get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Title { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool Physical { get; set; }
    public string? Company { get; set; }

    public bool IsLoyal { get; set; }
    public string Username { get; set; }
    public string AccountId { get; set; }
    public string BaseAddress { get; set; }

    public string FullName => Physical
        ? $"{FirstName} {LastName}".Trim()
        : (string.IsNullOrWhiteSpace(Company) ? $"{FirstName} {LastName}".Trim() : Company);

    public FrontendCustomerDTO() { }

    public FrontendCustomerDTO(CustomerApiDTO customer)
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
        IsLoyal = customer.IsLoyal ?? false;
        Username = customer.Username ?? string.Empty;
        AccountId = customer.AccountID?.ToString() ?? string.Empty;
        BaseAddress = customer.BaseAddress ?? string.Empty;
        City = customer.City;
        PostalCode = customer.PostalCode;
        Email = customer.Email;
        Phone = customer.Phone;
        Physical = customer.Physical;
        Company = customer.Company;
    }

    public CustomerApiDTO ToApiDTO()
    {
        return new CustomerApiDTO
        {
            Id = Id,
            FirstName = FirstName ?? string.Empty,
            LastName = LastName ?? string.Empty,
            Title = Title ?? string.Empty,
            Address = Address ?? string.Empty,
            City = City ?? string.Empty,
            PostalCode = PostalCode ?? string.Empty,
            Email = Email ?? string.Empty,
            Phone = Phone ?? string.Empty,
            Physical = Physical,
            Company = Company ?? string.Empty
        };
    }

    private FrontendCustomerDTO oldVersion;

    public void BeginEdit()
    {
        oldVersion = (FrontendCustomerDTO)this.MemberwiseClone();
    }

    public void CancelEdit()
    {
        if (oldVersion == null) return;

        this.Id = oldVersion.Id;
        this.FirstName = oldVersion.FirstName;
        this.LastName = oldVersion.LastName;
        this.Title = oldVersion.Title;
        this.Address = oldVersion.Address;
        this.City = oldVersion.City;
        this.PostalCode = oldVersion.PostalCode;
        this.Email = oldVersion.Email;
        this.Phone = oldVersion.Phone;
        this.Physical = oldVersion.Physical;
        this.Company = oldVersion.Company;
        this.IsLoyal = oldVersion.IsLoyal;
        this.BaseAddress = oldVersion.BaseAddress;
        this.AccountId = oldVersion.AccountId;
        this.Username = oldVersion.Username;
    }

    public void EndEdit()
    {
        oldVersion = null!;
    }
}
