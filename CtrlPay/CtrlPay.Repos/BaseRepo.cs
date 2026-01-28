using CtrlPay.Entities;
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
        string? json = await HttpGetter.HttpGet(url, ct);
        if (string.IsNullOrWhiteSpace(json)) return;
        try
        {
            var result = JsonSerializer.Deserialize<ReturnModel<List<TApiDto>>>(json, SerializerOptions);

            // Pokud je Body null, použijeme prázdný list, aby to nespadlo
            var apiList = result?.Body ?? [];

            // Atomická aktualizace - vytvoříme nový list a pak ho přiřadíme
            // Kdyby někdo zrovna četl Cache, aplikace nespadne
            Cache = [.. apiList.Select(mapper)];
            LastUpdated = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při parsování listu z {url}: {ex.Message}");
        }
    }

    // Společná metoda pro načtení sumy
    protected static async Task LoadSumFromApi(string url, CancellationToken ct)
    {
        string? json = await HttpGetter.HttpGet(url, ct);
        if (string.IsNullOrWhiteSpace(json)) return;

        try
        {
            // Tady sjednocujeme parsování přes ReturnModel
            var result = JsonSerializer.Deserialize<ReturnModel<decimal>>(json, SerializerOptions);
            SumCache = result?.Body ?? 0;
        }
        catch (Exception)
        {
            // Fallback pro případ, že API vrací jen holé číslo (staré API?)
            if (decimal.TryParse(json, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val))
            {
                SumCache = val;
            }
        }
    }

    // Společné řazení
    protected static List<FrontendTransactionDTO> SortData(List<FrontendTransactionDTO> data, string? sortingMethod)
    {
        if (sortingMethod != null) SortMethod = sortingMethod;
        string method = sortingMethod ?? SortMethod;

        return method switch
        {
            "AmountAsc" => [.. data.OrderBy(d => d.Amount)],
            "AmountDesc" => [.. data.OrderByDescending(d => d.Amount)],
            "DateAsc" => [.. data.OrderBy(d => d.Timestamp)],
            _ => [.. data.OrderByDescending(d => d.Timestamp)] // Default DateDesc
        };
    }
}