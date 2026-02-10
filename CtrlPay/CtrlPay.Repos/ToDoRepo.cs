using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public static class ToDoRepo
{
    public static string GetCreditAddress()
    {
        if (DebugMode.SkipCreditAddressLogic) return "DEBUG_address_for_credits";

        // sem dodat kód co vezme nejlépe uloženou adresu a pošlejí

        return "Testing";
    }
}
