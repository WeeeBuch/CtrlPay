using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public enum AuditLogStateEnum
    {
        Successful = 0,
        Failed = 1,
        Timeout = 2,
        Pending = 3
    }
}
