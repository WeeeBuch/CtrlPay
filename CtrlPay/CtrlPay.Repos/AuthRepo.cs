using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class AuthRepo
    {
        public static string ErrorMessage = "";

        public static bool Register(string username, string code, string password, string confirmPassword)
        {
            /* TODO: Register logic
             * Tu logika pro registraci
             *
             * Vrací true, pokud je registrace úspěšná, jinak false
             */

            #region Validations out of Api

            if (username == null || username.Trim() == "")
            {
                ErrorMessage = "Uživatelské jméno nesmí být prázdné.";
                return false;
            }

            if (password == null || password.Trim() == "")
            {
                ErrorMessage = "Heslo nesmí být prázdné.";
                return false;
            }

            if (confirmPassword == null || confirmPassword.Trim() == "")
            {
                ErrorMessage = "Potvrzení hesla nesmí být prázdné.";
                return false;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Hesla se neshodují.";
                return false;
            }

            if (code == null || code.Trim() == "")
            {
                ErrorMessage = "Registrační kód nesmí být prázdný.";
                return false;
            }

            #endregion

            return true;
        }

        public static bool Login(string username, string password)
        {
            /* TODO: Loginování logic
             * Tu logika pro přihlášení
             *
             * Vrací true, pokud je přihlášení úspěšné, jinak false
             */

            #region Validations out of Api

            if (username == null || username.Trim() == "")
            {
                ErrorMessage = "Uživatelské jméno nesmí být prázdné.";
                return false;
            }

            if (password == null || password.Trim() == "")
            {
                ErrorMessage = "Heslo nesmí být prázdné.";
                return false;
            }

            #endregion


            return true;
        }

        public static string RegisterFailedMessage() => ErrorMessage == "" ? "Něco je špatně. :)" : ErrorMessage;
        public static string LoginFailedMessage() => ErrorMessage == "" ? "Něco je špatně. :)" : ErrorMessage;
    }
}
