using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class ReturnModel
    {
        public string ReturnCode { get; set; }
        public ReturnModelSeverityEnum Severity { get; set; }
        public string BaseMessage { get; set; }
        public string DetailMessage { get; set; }
        protected static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
        {
            { "A0", "success" },
            { "A1", "username already exists" },
            { "A2", "incorrect password" },
            { "A3", "user does not exist" },
            { "A4", "invalid TOTP code" },
            { "A5", "login successful, awaiting TOTP verification" },
            { "A6", "user does not have account"   },
            { "A7", "invalid or expired token"   },
            { "A8", "token is not a TOTP token"   },
            { "A9", "HTTP error code received" },
            { "T0", "operation successful" },
            { "P0", "operation successful" }
        };

        public ReturnModel()
        {

        }
        public ReturnModel(string returnCode, ReturnModelSeverityEnum severity)
        {
            ReturnCode = returnCode;
            Severity = severity;
            SetMessage();
        }

        private void SetMessage()
        {
            BaseMessage = keyValuePairs[ReturnCode];
        }
    }
    public class ReturnModel<T> : ReturnModel
    {
        public string ReturnCode { get; set; }
        public ReturnModelSeverityEnum Severity { get; set; }
        public string BaseMessage { get; set; }
        public T Body { get; set; }
        public string DetailMessage { get; set; }
        private static Dictionary<string, string> keyValuePairs = ReturnModel.keyValuePairs;

        public ReturnModel()
        {
            
        }
        public ReturnModel(string returnCode, ReturnModelSeverityEnum severity, T body)
        {
            ReturnCode = returnCode;
            Severity = severity;
            Body = body;
            SetMessage();
        }

        private void SetMessage()
        {
            BaseMessage = keyValuePairs[ReturnCode];
        }
    }
    
}
