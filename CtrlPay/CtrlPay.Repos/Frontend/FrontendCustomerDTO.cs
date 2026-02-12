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
    public string? FirstName;
    public string? LastName;
    public string? Title;
    public string? Address;
    public string? City;
    public string? PostalCode;
    public string? Email;
    public string? Phone;

    public bool IsLoyal;
    public readonly string Username;
    public readonly string AccountId;
    public readonly string BaseAddress;

    public FrontendCustomerDTO(string firstName, string lastName, string title, string address, string city, string postalCode, string email, string phone, bool isLoyal, string username, string accountId, string baseAddress)
    {
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
