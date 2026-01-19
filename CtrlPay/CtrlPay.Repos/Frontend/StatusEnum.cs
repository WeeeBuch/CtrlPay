using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend
{
    public enum StatusEnum
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Confirmed = 3,
        Created = 4,
        WaitingForPayment = 5,
        PartiallyPaid = 6,
        Paid = 7,
        Overpaid = 8,
        Expired = 9,
        Cancelled = 10,
        Error = 11,
    }
}
