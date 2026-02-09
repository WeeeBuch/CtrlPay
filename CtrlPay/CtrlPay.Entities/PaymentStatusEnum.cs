using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public enum PaymentStatusEnum
    {
        Unpaid = 0,
        WaitingForPayment = 1,
        PartiallyPaid = 2,
        Paid = 3,
        Overpaid = 4,
        Expired = 5,
        Cancelled = 6
    }
}
