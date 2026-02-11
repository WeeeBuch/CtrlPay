using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public static class ToDoRepo
{
    public static List<Customer> GetCustomers()
    {

        return [
            new() {
                Id = 1,
                Address = "Test",
                City = "Testov",
                Email = "aa@bb.cz",
                FirstName = "Karel",
                LastName = "Lerak",
                Phone = "+420123456789",
                PostalCode = "12345",
                Title = "Mister"
            },
            new() {
                Id = 2,
                Address = "Test02",
                City = "Testov",
                Email = "aa@bb.cz",
                FirstName = "Karel",
                LastName = "Lerak",
                Phone = "+420123456789",
                PostalCode = "12345",
                Title = "Mister"
            },
            new() {
                Id = 3,
                Address = "Test03",
                City = "Testov",
                Email = "aa@bb.cz",
                FirstName = "Karel",
                LastName = "Lerak",
                Phone = "+420123456789",
                PostalCode = "12345",
                Title = "Mister"
            }
        ];
        
    }
}
