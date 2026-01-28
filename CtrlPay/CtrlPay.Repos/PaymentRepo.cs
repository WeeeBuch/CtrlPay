using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;

namespace CtrlPay.Repos;

public class PaymentRepo : BaseRepo<PaymentApiDTO>
{
    public static async Task UpdatePaymetsCacheFromApi(CancellationToken ct)
    {
        #region Debug
        if (DebugMode.MockPayments)
        {
            Cache = GetMockPayments();
            return;
        }
        #endregion

        await LoadListFromApi("/api/payments/my", p => new FrontendTransactionDTO(p), ct);
    }

    public static async Task UpdatePaymentSumCacheFromApi(CancellationToken ct)
    {
        if (DebugMode.MockPaymentSum) { SumCache = 500; return; }
        await LoadSumFromApi("/api/payments/amount-due", ct);
    }

    public static List<FrontendTransactionDTO> GetSortedDebts(string? sortingMethod, bool payable)
    {
        // Specifická logika pro Payments - filtrování
        // Pokud payable == true, vrátíme jen ty, na které máme dost peněz (podle TransactionRepo)
        var filtered = payable
            ? [.. Cache.Where(t => t.Amount <= TransactionRepo.GetTransactionSum())]
            : Cache;

        return SortData(filtered, sortingMethod);
    }

    public static decimal GetPaymentSum() => SumCache;

    private static List<FrontendTransactionDTO> GetMockPayments() =>
    [
        new() { Title = "Debug Debt 1", Amount = 52, Timestamp = DateTime.UtcNow, State = StatusEnum.Pending, Id = 1 },
        new() { Title = "Debug Debt 2", Amount = 21, Timestamp = DateTime.UtcNow.AddDays(-1), State = StatusEnum.PartiallyPaid, Id = 2 },
        new() { Title = "Debug Debt 3", Amount = 123.45m, Timestamp = DateTime.UtcNow.AddDays(-2), State = StatusEnum.Completed, Id = 3 },
        new() { Title = "Debug Debt 4", Amount = 45, Timestamp = DateTime.UtcNow.AddDays(-3), State = StatusEnum.Confirmed, Id = 4 },
        new() { Title = "Debug Debt 5", Amount = 75, Timestamp = DateTime.UtcNow.AddDays(-7), State = StatusEnum.Expired, Id = 5 },
        new() { Title = "Debug Debt 6", Amount = 133, Timestamp = DateTime.UtcNow.AddDays(-8), State = StatusEnum.Overpaid, Id = 6 },
        new() { Title = "Debug Debt 7", Amount = 454, Timestamp = DateTime.UtcNow.AddDays(-9), State = StatusEnum.WaitingForPayment, Id = 7 },
        new() { Title = "Debug Debt 8", Amount = 735, Timestamp = DateTime.UtcNow.AddDays(-14), State = StatusEnum.Paid, Id = 8 },
        new() { Title = "Debug Debt 9", Amount = 486, Timestamp = DateTime.UtcNow.AddDays(-15), State = StatusEnum.Cancelled, Id = 9 }
    ];
}