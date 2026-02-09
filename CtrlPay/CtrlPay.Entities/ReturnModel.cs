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
            { "A10", "username can not be blank" },
            { "A11", "password can not be blank" },
            { "A12", "http return is null" },
            { "A13", "failed to parse http response" },
            { "C0", "One time address created successfully" },
            { "H0", "i'm alive" },
            { "P0", "operation successful" },
            { "P1", "user does not have account" },
            { "P2", "payment not found" },
            { "P3", "user does not have permission to pay payment" },
            { "P4", "payment is already paid" },
            { "R0", "successfully registered" },
            { "R1", "username can not be empty" },
            { "R2", "password can not be empty" },
            { "R3", "confirm password can not be empty" },
            { "R4", "passwords do not match" },
            { "R5", "code can not be empty" },
            { "T0", "operation successful" },
            { "T1", "user does not have account" },
        };

        public ReturnModel(string returnCode, ReturnModelSeverityEnum severity)
        {
            ReturnCode = returnCode;
            Severity = severity;
            BaseMessage = keyValuePairs[ReturnCode];
            DetailMessage = "";
        }
    }

    public class ReturnModel<T> : ReturnModel
    {
        public T Body { get; set; }

        public ReturnModel(string returnCode, ReturnModelSeverityEnum severity, T body) : base(returnCode, severity)
        {
            Body = body;
        }
    }
    
}
