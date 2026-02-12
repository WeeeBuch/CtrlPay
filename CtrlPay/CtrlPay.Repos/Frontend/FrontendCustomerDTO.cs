using CtrlPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend;

public class FrontendCustomerDTO
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

    public bool IsLoyal { get; set; }
    public string Username { get; set; }
    public string AccountId { get; set; }
    public string BaseAddress { get; set; }

    public FrontendCustomerDTO() { }

    public FrontendCustomerDTO(int id, string firstName, string lastName, string title, string address, string city, string postalCode, string email, string phone, bool isLoyal, string username, string accountId, string baseAddress)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Title = title;
        Address = address;
        City = city;
        PostalCode = postalCode;
        Email = email;
        Phone = phone;
        IsLoyal = isLoyal;
        Username = username;
        AccountId = accountId;
        BaseAddress = baseAddress;
    }

    public FrontendCustomerDTO(Customer customer, bool isLoyal, string username, string accountId, string baseAddress)
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
        IsLoyal = isLoyal;
        Username = username;
        AccountId = accountId;
        BaseAddress = baseAddress;
    }
}
