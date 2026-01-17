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
        public JWT? JWT { get; set; }
        public string Message { get; set; }

        public AuthLogicReturnModel(int returnCode, string message)
        {
            ReturnCode = returnCode;
            Message = message;
        }
        public AuthLogicReturnModel(int returnCode, string message, JWT jwt)
        {
            ReturnCode = returnCode;
            Message = message;
            JWT = jwt;
        }
    }
}
