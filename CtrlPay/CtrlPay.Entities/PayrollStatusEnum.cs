using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public enum PayrollStatusEnum
    {
        Created = 0,
        ApprovedByAccountant = 1,
        ApprovedBySupervisor = 2,
        ReadyForPayday = 3,
        Paid = 4,
        Rejected = 4,
        Cancelled = 5
    }
}
