using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static class AuthRepo
    {
        public static bool Register(string username, string email, string password, string confirmPassword)
        {
            /* TODO: Register logic
             * Tu logika pro registraci
             *
             * Vrací true, pokud je registrace úspěšná, jinak false
             */
            return true;
        }
        public static string RegisterFailedMessage()
        {
            /* TODO: Register failed logic
             * Zde se vrátí message jestli je třeba heslo špatně nebo jménu atd.
             */
            return "Registrace se nezdařila. Zkontrolujte něco co se nepovedlo.";
        }
        public static bool Login(string username, string password)
        {
            /* TODO: Loginování logic
             * Tu logika pro přihlášení
             *
             * Vrací true, pokud je přihlášení úspěšné, jinak false
             */
            return true;
        }

        public static string LoginFailedMessage()
        {
            /* TODO: login failed logic
             * Zde se vrátí message jestli je třeba heslo špatně nebo jménu atd.
             */
            return "Přihlášení se nezdařilo. Zkontrolujte něco co se nepovedlo.";
        }
    }
}
