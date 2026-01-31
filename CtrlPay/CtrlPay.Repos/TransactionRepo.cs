using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;

namespace CtrlPay.Repos;

public class TransactionRepo : BaseRepo<TransactionApiDTO>
{
    public static async Task UpdateTransactionsCacheFromApi(CancellationToken ct)
    {
        #region Debug
        if (DebugMode.MockTransactions)
        {
            Cache = GetMockTransactions();
            return;
        }
        #endregion

        // Voláme metodu z BaseRepo, řekneme URL a jak převést data (t => new(t))
        await LoadListFromApi("/api/transactions/my", t => new FrontendTransactionDTO(t), ct);
    }

    public static async Task UpdateTransactionSumCacheFromApi(CancellationToken ct)
    {
        if (DebugMode.MockTransactionSum) { SumCache = 1234; return; }
        await LoadSumFromApi("/api/transactions/credit", ct);
    }

    public static List<FrontendTransactionDTO> GetSortedTransactions(string? sortingMethod)
    {
        return SortData(Cache, sortingMethod);
    }

    public static decimal GetTransactionSum() => SumCache;

    // Tvůj mock helper (zkráceno pro přehlednost, klidně si tam nech ten svůj dlouhý list)
    private static List<FrontendTransactionDTO> GetMockTransactions() =>
    [
        new() { Title = "Debug Transaction 1", Amount = 123.45m, Timestamp = DateTime.UtcNow, State = StatusEnum.Pending, Id = 1 },
        new() { Title = "Debug Transaction 2", Amount = 42, Timestamp = DateTime.UtcNow.AddDays(-1), State = StatusEnum.PartiallyPaid, Id = 2 },
        new() { Title = "Debug Transaction 3", Amount = 453, Timestamp = DateTime.UtcNow.AddDays(-2), State = StatusEnum.Completed, Id = 3 },
        new() { Title = "Debug Transaction 4", Amount = 45, Timestamp = DateTime.UtcNow.AddDays(-3), State = StatusEnum.Confirmed, Id = 4 },
        new() { Title = "Debug Transaction 5", Amount = 213, Timestamp = DateTime.UtcNow.AddDays(-7), State = StatusEnum.Expired, Id = 5 },
        new() { Title = "Debug Transaction 6", Amount = 354, Timestamp = DateTime.UtcNow.AddDays(-8), State = StatusEnum.Overpaid, Id = 6 },
        new() { Title = "Debug Transaction 7", Amount = 73, Timestamp = DateTime.UtcNow.AddDays(-9), State = StatusEnum.WaitingForPayment, Id = 7 },
        new() { Title = "Debug Transaction 8", Amount = 321, Timestamp = DateTime.UtcNow.AddDays(-14), State = StatusEnum.Paid, Id = 8 },
        new() { Title = "Debug Transaction 9", Amount = 05, Timestamp = DateTime.UtcNow.AddDays(-15), State = StatusEnum.Cancelled, Id = 9 }
    ];
}