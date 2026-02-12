using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public enum Role
    {
        Customer = 0,
        Admin = 1,
        Accountant = 2,
        PayrollAcountant = 3,
        Employee = 4
    }
}
