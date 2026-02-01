using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Avalonia;

public static class UpdateHandler
{
    // Cradits available update
    public static List<Action<decimal>> CreditAvailableUpdateActions = [];
    public static void HandleCreditAvailableUpdate(decimal newAmount)
        => CreditAvailableUpdateActions.ForEach(action => action.Invoke(newAmount));

    // Pending payments update
    public static List<Action<decimal>> PendingPaymentsUpdateActions = [];
    public static void HandlePendingPaymentsUpdate(decimal newAmount)
        => PendingPaymentsUpdateActions.ForEach(action => action.Invoke(newAmount));

    // New payments added
    public static List<Action> NewPaymentsAddedActions = [];
    public static void HandleNewPaymentsAdded()
        => NewPaymentsAddedActions.ForEach(action => action.Invoke());

    // New debts added
    public static List<Action> NewDebtsAddedActions = [];
    public static void HandleNewDebtsAdded()
        => NewDebtsAddedActions.ForEach(action => action.Invoke());

}

