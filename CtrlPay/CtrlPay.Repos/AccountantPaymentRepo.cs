using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class AccountantPaymentRepo
    {
        // Data jsou statická, ale unikátní pro každý typ TApiDto (takže se nemíchají)
        protected static List<FrontendPaymentDTO> Cache { get; set; } = [];
        protected static DateTime LastUpdated { get; set; } = DateTime.MinValue;
        protected static string SortMethod = "DateDesc";
        protected static JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

        // Společná metoda pro načtení seznamu
        protected static async Task LoadListFromApi(
            string url,
            Func<PaymentApiDTO, FrontendPaymentDTO> mapper, // Funkce pro převod DTO
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
                var result = JsonSerializer.Deserialize<ReturnModel<List<PaymentApiDTO>>>(json, SerializerOptions);

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

        // Společné řazení
        protected static List<FrontendPaymentDTO> SortData(List<FrontendPaymentDTO> data, string? sortingMethod)
        {
            if (sortingMethod != null) SortMethod = sortingMethod;
            string method = sortingMethod ?? SortMethod;

            AppLogger.Info($"Sorting data by: {method}");

            return method switch
            {
                "AmountAsc" => [.. data.OrderBy(d => d.PaidAmountXMR)],
                "AmountDesc" => [.. data.OrderByDescending(d => d.PaidAmountXMR)],
                "DateAsc" => [.. data.OrderBy(d => d.CreatedAt)],
                _ => [.. data.OrderByDescending(d => d.CreatedAt)] // Default DateDesc
            };
        }
        public static async Task UpdatePaymetsCacheFromApi(CancellationToken ct)
        {
            AppLogger.Info($"Updating Cached Payments...");
            #region Debug
            if (DebugMode.MockPaymentManager)
            {
                AppLogger.Info($"Returning Mock payments...");
                Cache = GetMockPayments();
                return;
            }
            #endregion

            await LoadListFromApi("/api/payments/all", p => new FrontendPaymentDTO(p), ct);
        }
        private static List<FrontendPaymentDTO> GetMockPayments() =>
        [
            new FrontendPaymentDTO
            {
                Id = 1,
                CustomerId = 1,
                ExpectedAmountXMR = 1.50m,
                PaidAmountXMR = 0m,
                Status = StatusEnum.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(5)
            },
            new FrontendPaymentDTO
            {
                Id = 2,
                CustomerId = 1,
                ExpectedAmountXMR = 0.75m,
                PaidAmountXMR = 0.30m,
                Status = StatusEnum.PartiallyPaid,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(6)
            },
            new FrontendPaymentDTO
            {
                Id = 3,
                CustomerId = 1,
                ExpectedAmountXMR = 2.00m,
                PaidAmountXMR = 2.00m,
                Status = StatusEnum.Paid,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                PaidAt = DateTime.UtcNow.AddDays(-4),
                DueDate = DateTime.UtcNow.AddDays(2)
            },
            new FrontendPaymentDTO  
            {
                Id = 4,
                CustomerId = 1,
                ExpectedAmountXMR = 1.20m,
                PaidAmountXMR = 1.50m,
                Status = StatusEnum.Overpaid,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                PaidAt = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(4)
            },
            new FrontendPaymentDTO
            {
                Id = 5,
                CustomerId = 1,
                ExpectedAmountXMR = 3.00m,
                PaidAmountXMR = 0m,
                Status = StatusEnum.Expired,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(-2)
            },
            new FrontendPaymentDTO
            {
                Id = 6,
                CustomerId = 1,
                ExpectedAmountXMR = 0.90m,
                PaidAmountXMR = 0m,
                Status = StatusEnum.Cancelled,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                DueDate = DateTime.UtcNow.AddDays(3)
            },
            new FrontendPaymentDTO
            {
                Id = 7,
                CustomerId = 1,
                ExpectedAmountXMR = 1.10m,
                PaidAmountXMR = 0m,
                Status = StatusEnum.WaitingForPayment,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            }
        ];

        public static List<FrontendPaymentDTO> GetSortedPayments(string? sortingMethod)
        {
            return Cache;
        }

    }
}
