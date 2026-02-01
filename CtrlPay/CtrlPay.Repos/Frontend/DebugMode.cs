using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos.Frontend;

public static class DebugMode
{
    public static bool StartDebug { get; set; } = true;

    // Skip loginu a Registeru
    public static bool SkipAuthReg { get; set; } = false;
    public static bool SkipAuthLogin { get; set; } = false;

    // Mock Transactions
    public static bool MockTransactionSum { get; set; } = false;
    public static bool MockTransactions { get; set; } = false;

    // Mock Payments
    public static bool MockPaymentSum { get; set; } = false;
    public static bool MockPayments { get; set; } = false;

    // Connection test
    public static bool SkipApiConnectionTest { get; set; } = false;

    // One time address generation skip
    public static bool SkipOneTimeAddressGeneration { get; set; } = false;

    public static List<DebugProperty> GetDebugProperties()
    {
        return typeof(DebugMode)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(bool) && p.Name != nameof(StartDebug))
            .Select(p => new DebugProperty(p))
            .ToList();
    }
}

public class DebugProperty
{
    private PropertyInfo _info;
    public string Name { get; }
    public bool Value
    {
        get => (bool)_info.GetValue(null)!;
        set => _info.SetValue(null, value);
    }

    public DebugProperty(PropertyInfo info)
    {
        _info = info;
        Name = info.Name;
    }
}
