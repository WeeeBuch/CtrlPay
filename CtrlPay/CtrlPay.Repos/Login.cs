using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public static partial class Repo
    {
        public static bool Login(string username, string password)
        {
            /*
             * Tu logika pro přihlášení
             *
             * Vrací true, pokud je přihlášení úspěšné, jinak false
             */
            return true;
        }

        public static string LoginFailedMessage()
        {
            /* 
             * Zde se vrátí message jestli je třeba heslo špatně nebo jménu atd.
             */
            return "Přihlášení se nezdařilo. Zkontrolujte něco co se nepovedlo.";
        }
    }
}
