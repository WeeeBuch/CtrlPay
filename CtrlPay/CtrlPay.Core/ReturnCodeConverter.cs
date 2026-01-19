using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Core
{
    public static class ReturnCodeConverter
    {
        private static Dictionary<int, string> keyValuePairs = new Dictionary<int, string>
        {
            { 0, "success" },
            { 1, "username already exists" },
            { 2, "incorrect password" },
            { 3, "user does not exist" },
            { 4, "invalid TOTP code" },
            { 5, "login successful, awaiting TOTP verification" }
        };

        public static string GetMessage(int code)
        {
            return keyValuePairs[code];
        }
    }
}
