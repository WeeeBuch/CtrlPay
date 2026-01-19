using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class AuthLogicReturnModel
    {
        public int ReturnCode { get; set; }
        public string Message { get; set; }

        public AuthLogicReturnModel(int returnCode, string message)
        {
            ReturnCode = returnCode;
            Message = message;
        }
    }
}
