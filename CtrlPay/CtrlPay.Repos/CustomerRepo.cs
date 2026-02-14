using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public class CustomerRepo
{
    private static List<FrontendCustomerDTO> Cache { get; set; } = [];
    private static DateTime LastUpdated { get; set; } = DateTime.MinValue;
    private static JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };
    public static async Task UpdateCustomersFromApi(CancellationToken ct)
    {
        AppLogger.Info($"Updating Cached Customers...");
        #region Debug
        if (DebugMode.MockPayments)
        {
            AppLogger.Info($"Returning Mock customers...");
            Cache = [
            new() {
            Id = 1,
            Address = "Test",
            City = "Testov",
            Email = "aa@bb.cz",
            FirstName = "Karel",
            LastName = "Lerak",
            Phone = "+420123456789",
            PostalCode = "12345",
            Title = "Mister"
        },
        new() {
            Id = 2,
            Address = "Test02",
            City = "Testov",
            Email = "aa@bb.cz",
            FirstName = "Karel",
            LastName = "Lerak",
            Phone = "+420123456789",
            PostalCode = "12345",
            Title = "Mister"
        },
        new() {
            Id = 3,
            Address = "Test03",
            City = "Testov",
            Email = "aa@bb.cz",
            FirstName = "Karel",
            LastName = "Lerak",
            Phone = "+420123456789",
            PostalCode = "12345",
            Title = "Mister"
        }
        ];
            return;
        }
        #endregion

        AppLogger.Info($"Getting json from API...");
        string? json = await HttpWorker.HttpGet("api/customers/all", true, ct);
        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Get response was NULL.");
            return;
        }

        try
        {
            AppLogger.Info($"Deserializing response...");
            var result = JsonSerializer.Deserialize<ReturnModel<List<CustomerApiDTO>>>(json, SerializerOptions);

            // Pokud je Body null, použijeme prázdný list, aby to nespadlo
            var apiList = result?.Body ?? [];

            // Atomická aktualizace - vytvoříme nový list a pak ho přiřadíme
            // Kdyby někdo zrovna četl Cache, aplikace nespadne
            Cache = [.. apiList.Select(c => new FrontendCustomerDTO(c))];
            LastUpdated = DateTime.UtcNow;
            AppLogger.Info($"Cached Transactions updated at {LastUpdated}.");
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Transaction list parsing failed.", ex);
        }
    }
    public static List<FrontendCustomerDTO> GetCustomers() => Cache;

    public static async Task DeleteCustomer(FrontendCustomerDTO cust)
    {
        Cache.Remove(cust);
        AppLogger.Info($"Deleting customer from API...");
        string? json = await HttpWorker.HttpDelete($"api/customers/delete/{cust.Id}", "", true, default);
        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Get response was NULL.");
            return;
        }
    }

    public static async Task UpdateCustomer(FrontendCustomerDTO cust)
    {
        AppLogger.Info($"Deleting customer from API...");
        string? json = await HttpWorker.HttpPost($"api/customers/edit", cust.ToApiDTO(), true, default);
        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Get response was NULL.");
            return;
        }
    }
}