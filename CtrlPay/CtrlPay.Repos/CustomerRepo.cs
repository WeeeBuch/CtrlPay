using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        if (DebugMode.MockCustomers)
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
                Title = "Mister",
                Physical = true,
                Company = "Test"
            }];

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

    public static List<FrontendCustomerDTO> Filter(string input)
    {
        return Cache.Where(c =>
            (c.FirstName != null && c.FirstName.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
            (c.LastName != null && c.LastName.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
            (c.Email != null && c.Email.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
            (c.Phone != null && c.Phone.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
            (c.Company != null && c.Company.Contains(input, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

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
        bool valid = ValidateCustomer(cust);
        if(!valid)
        {
            //TODO: vyhodit něco uživateli, že data jsou nevalidní
            AppLogger.Warning($"Customer data is invalid. Update aborted.");
            return;
        }
        int id = Cache.FindIndex(c => c.Id == cust.Id);
        if (id == -1)
        {
            AppLogger.Info($"Adding customer to API...");
            string? json = await HttpWorker.HttpPost($"api/customers/create", cust.ToApiDTO(), true, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Get response was NULL.");
                return;
            }
            Cache.Add(cust);
        }
        else
        {
            Cache[id] = cust;
            AppLogger.Info($"Updating customer in API...");
            string? json = await HttpWorker.HttpPost($"api/customers/edit", cust.ToApiDTO(), true, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Get response was NULL.");
                return;
            }
        }
        
    }

    private static bool ValidateCustomer(FrontendCustomerDTO cust)
    {
        bool isValid = true;
        // ===== POVINNÁ POLE =====
        if (string.IsNullOrWhiteSpace(cust.Company) || (!string.IsNullOrWhiteSpace(cust.FirstName) && !string.IsNullOrWhiteSpace(cust.LastName)))
            isValid = false;

        // ===== MAXIMÁLNÍ DÉLKY =====
        if (!string.IsNullOrEmpty(cust.FirstName) && cust.FirstName.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.LastName) && cust.LastName.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Title) && cust.Title.Length > 10)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Address) && cust.Address.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.PostalCode) && cust.PostalCode.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.City) && cust.City.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Email) && cust.Email.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Phone) && cust.Phone.Length > 255)
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Company) && cust.Company.Length > 255)
            isValid = false;

        // ===== FORMÁT =====
        if (!string.IsNullOrEmpty(cust.Email) &&
            !new EmailAddressAttribute().IsValid(cust.Email))
            isValid = false;

        if (!string.IsNullOrEmpty(cust.PostalCode) &&
            !Regex.IsMatch(cust.PostalCode, @"^\d{5}$"))
            isValid = false;

        if (!string.IsNullOrEmpty(cust.Phone) &&
            !Regex.IsMatch(cust.Phone, @"^[0-9+\s]+$"))
            isValid = false;

        // ===== BUSINESS LOGIKA =====
        if (cust.Physical && !string.IsNullOrEmpty(cust.Company))
            isValid = false;

        if (!cust.Physical && string.IsNullOrWhiteSpace(cust.Company))
            isValid = false;
        return isValid;
    }
}