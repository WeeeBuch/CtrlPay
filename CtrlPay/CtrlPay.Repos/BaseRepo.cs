using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System.Text.Json;

namespace CtrlPay.Repos;

// TApiDto je typ objektu, který ti vrací API (TransactionApiDTO nebo PaymentApiDTO)
public abstract class BaseRepo<TApiDto>
{
    // Data jsou statická, ale unikátní pro každý typ TApiDto (takže se nemíchají)
    protected static List<FrontendTransactionDTO> Cache { get; set; } = [];
    protected static decimal SumCache { get; set; } = 0;
    protected static DateTime LastUpdated { get; set; } = DateTime.MinValue;
    protected static string SortMethod = "DateDesc";
    protected static JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    // Společná metoda pro načtení seznamu
    protected static async Task LoadListFromApi(
        string url,
        Func<TApiDto, FrontendTransactionDTO> mapper, // Funkce pro převod DTO
        CancellationToken ct)
    {
        AppLogger.Info($"Getting json from API...");
        string? json = await HttpWorker.HttpGet(url, true, ct);
        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Get response was NULL.");
            return;
        }

        try
        {
            AppLogger.Info($"Deserializing response...");
            var result = JsonSerializer.Deserialize<ReturnModel<List<TApiDto>>>(json, SerializerOptions);

            // Pokud je Body null, použijeme prázdný list, aby to nespadlo
            var apiList = result?.Body ?? [];

            // Atomická aktualizace - vytvoříme nový list a pak ho přiřadíme
            // Kdyby někdo zrovna četl Cache, aplikace nespadne
            Cache = [.. apiList.Select(mapper)];
            LastUpdated = DateTime.UtcNow;
            AppLogger.Info($"Cached Transactions updated at {LastUpdated}.");
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Transaction list parsing failed.", ex);
        }
    }

    // Společná metoda pro načtení sumy
    protected static async Task LoadSumFromApi(string url, CancellationToken ct)
    {
        AppLogger.Info($"Getting Sums from API...");
        string? json = await HttpWorker.HttpGet(url, true, ct);
        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Get response was NULL.");
            return;
        }

        try
        {
            AppLogger.Info($"Deserializing...");
            // Tady sjednocujeme parsování přes ReturnModel
            var result = JsonSerializer.Deserialize<ReturnModel<decimal>>(json, SerializerOptions);
            SumCache = result?.Body ?? -1;
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Sums load failed.", ex);
        }
    }

    // Společné řazení
    protected static List<FrontendTransactionDTO> SortData(List<FrontendTransactionDTO> data, string? sortingMethod)
    {
        if (sortingMethod != null) SortMethod = sortingMethod;
        string method = sortingMethod ?? SortMethod;

        AppLogger.Info($"Sorting data by: {method}");

        return method switch
        {
            "AmountAsc" => [.. data.OrderBy(d => d.Amount)],
            "AmountDesc" => [.. data.OrderByDescending(d => d.Amount)],
            "DateAsc" => [.. data.OrderBy(d => d.Timestamp)],
            _ => [.. data.OrderByDescending(d => d.Timestamp)] // Default DateDesc
        };
    }
}