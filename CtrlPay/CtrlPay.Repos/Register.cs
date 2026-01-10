using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static partial class Repo
    {
        public static bool Register(string username, string email, string password, string confirmPassword)
        {
            /*
             * Tu logika pro registraci
             *
             * Vrací true, pokud je registrace úspěšná, jinak false
             */
            return true;
        }
        public static string RegisterFailedMessage()
        {
            /* 
             * Zde se vrátí message jestli je třeba heslo špatně nebo jménu atd.
             */
            return "Registrace se nezdařila. Zkontrolujte něco co se nepovedlo.";
        }
    }
}
