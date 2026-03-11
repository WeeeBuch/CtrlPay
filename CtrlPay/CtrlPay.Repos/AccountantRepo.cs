using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class AccountantRepo
    {
        protected static List<FrontendPaymentDTO> PaymentCache { get; set; } = [];
        protected static List<AccountantTransactionDTO> TransactionCache { get; set; } = [];
        protected static DateTime LastUpdated { get; set; } = DateTime.MinValue;
        protected static string SortMethod = "DateDesc";
        protected static JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

        // Společná metoda pro načtení seznamu
        protected static async Task LoadPaymentListFromApi(
            string url,
            Func<PaymentApiDTO, FrontendPaymentDTO> mapper,
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
                PaymentCache = [.. apiList.Select(mapper)];
                LastUpdated = DateTime.UtcNow;
                AppLogger.Info($"Cached Payments updated at {LastUpdated}.");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Payment list parsing failed.", ex);
            }
        }
        protected static async Task LoadTransactionListFromApi(
            string url,
            Func<TransactionApiDTO, AccountantTransactionDTO> mapper,
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
                var result = JsonSerializer.Deserialize<ReturnModel<List<TransactionApiDTO>>>(json, SerializerOptions);

                // Pokud je Body null, použijeme prázdný list, aby to nespadlo
                var apiList = result?.Body ?? [];

                // Atomická aktualizace - vytvoříme nový list a pak ho přiřadíme
                // Kdyby někdo zrovna četl Cache, aplikace nespadne
                TransactionCache = [.. apiList.Select(mapper)];
                LastUpdated = DateTime.UtcNow;
                AppLogger.Info($"Cached Transactions updated at {LastUpdated}.");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Transaction list parsing failed.", ex);
            }
        }


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
        public static async Task UpdateAccountantCachesFromApi(CancellationToken ct)
        {
            AppLogger.Info($"Updating Cached Payments...");
            #region Debug
#if DEBUG
            if (DebugMode.MockPaymentManager)
            {
                AppLogger.Info($"Returning Mock payments...");
                PaymentCache = GetMockPayments();
                return;
            }
#endif
            #endregion

            await LoadPaymentListFromApi("/api/payments/all", p => new FrontendPaymentDTO(p), ct);
            await LoadTransactionListFromApi("/api/transactions/all", t => new AccountantTransactionDTO(t), ct);
        }

        public static List<FrontendPaymentDTO> GetPaymentsByStatus(StatusEnum state) => PaymentCache.Where(p => p.Status == state).ToList();
#if DEBUG
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
#endif
        public static List<FrontendPaymentDTO> GetSortedPayments(string? sortingMethod)
        {
            return SortData(PaymentCache, sortingMethod);
        }
        public static async Task UpdatePayment(FrontendPaymentDTO dto)
        {
            int id = PaymentCache.FindIndex(c => c.Id == dto.Id);
            if (id == -1)
            {
                AppLogger.Info($"Adding payment to API...");
                string? json = await HttpWorker.HttpPost($"api/payments/create", dto.ToApiDto(), true, default);
                if (string.IsNullOrWhiteSpace(json))
                {
                    AppLogger.Warning($"Create payment response was NULL.");
                    return;
                }
                PaymentCache.Add(dto);
            }
            else
            {
                PaymentCache[id] = dto;
                AppLogger.Info($"Updating payment in API...");
                string? json = await HttpWorker.HttpPost($"api/payments/update", dto.ToApiDto(), true, default);
                if (string.IsNullOrWhiteSpace(json))
                {
                    AppLogger.Warning($"Update payment response was NULL.");
                    return;
                }
            }
        }
        public static async Task DeletePayment(FrontendPaymentDTO toDelete)
        {
            PaymentCache.Remove(toDelete);
            AppLogger.Info($"Deleting payment in API...");
            string? json = await HttpWorker.HttpDelete($"/api/payments/delete/{toDelete.Id}", true);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Delete payment response was NULL.");
                return;
            }
        }
        public static async Task<bool> ConvertOverpaymentToCredit(FrontendPaymentDTO payment)
        {
            decimal surplus = payment.PaidAmountXMR - payment.ExpectedAmountXMR;
            if (surplus <= 0)
            {
                AppLogger.Warning($"Payment {payment.Id} nemá žádný přebytek.");
                return false;
            }

            AppLogger.Info($"Převod přebytku {surplus} XMR do kreditů pro zákazníka {payment.CustomerId}...");

            // TODO: Karele toto
            string? json = await HttpWorker.HttpPost("api/payments/overpay-to-credit", payment.ToApiDto(), true, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Convert-to-credit response byl NULL.");
                return false;
            }

            // Lokálně aktualizujeme cache – přebytek se "vyčerpá", status se změní
            int idx = PaymentCache.FindIndex(c => c.Id == payment.Id);
            if (idx != -1)
            {
                PaymentCache[idx].PaidAmountXMR = PaymentCache[idx].ExpectedAmountXMR; // přebytek pryč
                PaymentCache[idx].Status = StatusEnum.Paid;
            }

            return true;
        }
#if DEBUG
        private static List<AccountantTransactionDTO> GetMockAccountantTransactions()
        {
            var rng = new Random();
            var customers = new[] { "Alice Corp.", "Bob's Burgers", "Charlie Chap", "Delta Force", "Echo Base", "Cyberdyne Systems", "Initech", "Umbrella Corp" };
            var types = new[] { TransactionTypeEnum.Incoming, TransactionTypeEnum.Outgoing, TransactionTypeEnum.Internal };
            var statuses = new[] { StatusEnum.Completed, StatusEnum.Pending, StatusEnum.Cancelled, StatusEnum.Confirmed, StatusEnum.Overpaid, StatusEnum.Created, StatusEnum.Failed, StatusEnum.WaitingForPayment, StatusEnum.PartiallyPaid, StatusEnum.Paid, StatusEnum.Expired };

            var list = new List<AccountantTransactionDTO>();
            for (int i = 0; i < 50; i++)
            {
                // Generování 64-znakového Monero TX Hashe
                byte[] hashBytes = new byte[32];
                rng.NextBytes(hashBytes);
                string xmrHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Generování Monero částky (0.000000000001 až 10.0 XMR)
                // Použijeme náhodné piconera a převedeme na decimal
                long piconero = (long)(rng.NextDouble() * 10000000000000L); // až 10 XMR
                decimal amount = (decimal)piconero / 1000000000000m;

                list.Add(new AccountantTransactionDTO
                {
                    Id = i + 1,
                    Title = xmrHash,
                    CustomerName = customers[rng.Next(customers.Length)],
                    Amount = amount,
                    Timestamp = DateTimeOffset.UtcNow.AddDays(-rng.Next(30)).AddMinutes(-rng.Next(1440)),
                    State = statuses[rng.Next(statuses.Length)],
                    Type = types[rng.Next(types.Length)]
                });
            }
            return [.. list.OrderByDescending(t => t.Timestamp)];
        }
#endif
        public static List<AccountantTransactionDTO> GetAccountantTransactions()
        {
#if DEBUG
            if (DebugMode.MockAccountantTransactions) return GetMockAccountantTransactions();
#endif
            return TransactionCache;
        }
        public static AccountantDashboardSummaryDTO GetAccountantDashboardSummary()
        {
#if DEBUG
            if (DebugMode.MockAccountantTransactions)
            {
                return new AccountantDashboardSummaryDTO
                {
                    OverpaidAmount = 1.25m,
                    OverpaidCount = 4,
                    OverdueAmount = 5.80m,
                    OverdueCount = 12,
                    PartiallyPaidAmount = 0.45m,
                    PartiallyPaidCount = 2,
                    WaitingAmount = 15.20m,
                    WaitingCount = 25
                };
            }
#endif
            return new AccountantDashboardSummaryDTO
            {
                OverpaidAmount = PaymentCache.Where(p => p.Status == StatusEnum.Overpaid).Sum(p => p.PaidAmountXMR - p.ExpectedAmountXMR),
                OverpaidCount = PaymentCache.Where(p => p.Status == StatusEnum.Overpaid).Count(),
                OverdueAmount = PaymentCache.Where(p => p.Status == StatusEnum.Expired).Sum(p => p.ExpectedAmountXMR - p.PaidAmountXMR),
                OverdueCount = PaymentCache.Where(p => p.Status == StatusEnum.Expired).Count(),
                PartiallyPaidAmount = PaymentCache.Where(p => p.Status == StatusEnum.PartiallyPaid).Sum(p => p.PaidAmountXMR),
                PartiallyPaidCount = PaymentCache.Where(p => p.Status == StatusEnum.PartiallyPaid).Count(),
                WaitingAmount = PaymentCache.Where(p => p.Status == StatusEnum.Pending || p.Status == StatusEnum.WaitingForPayment).Sum(p => p.ExpectedAmountXMR),
                WaitingCount = PaymentCache.Where(p => p.Status == StatusEnum.Pending || p.Status == StatusEnum.WaitingForPayment).Count()
            };
        }
        public static AccountantChartDataDTO GetAccountantChartData()
        {
            var data = new AccountantChartDataDTO();
#if DEBUG
            if (DebugMode.MockAccountantTransactions)
            {
                var rng = new Random();

                // Generujeme 30 dní historie příjmů
                for (int i = 30; i >= 0; i--)
                {
                    data.IncomeHistory.Add(new IncomeChartPointDTO
                    {
                        Date = DateTime.Today.AddDays(-i),
                        Amount = (decimal)(rng.NextDouble() * 2.5) // Náhodné příjmy 0-2.5 XMR
                    });
                }

                data.StatusBreakdown = new List<(string Status, int Count)>
                {
                    ("Paid", 145),
                    ("Overpaid", 12),
                    ("PartiallyPaid", 8),
                    ("Expired", 24)
                };
                return data;
            }
#endif
            for (int i = 30; i >= 0; i--)
            {
                DateTime date = DateTime.Today.AddDays(-i);
                data.IncomeHistory.Add(new IncomeChartPointDTO
                {
                    Date = date,
                    Amount = TransactionCache.Where(t => t.Timestamp == date).Sum(t => t.Amount),
                });
            }

            List<FrontendPaymentDTO> filteredCache = PaymentCache.Where(t => (DateTimeOffset.Now - t.CreatedAt).TotalDays <= 30).ToList();

            data.StatusBreakdown = new List<(string Status, int Count)>
            {
                ("Paid", filteredCache.Count(p => p.Status == StatusEnum.Paid)),
                ("Overpaid", filteredCache.Count(p => p.Status == StatusEnum.Overpaid)),
                ("PartiallyPaid", filteredCache.Count(p => p.Status == StatusEnum.PartiallyPaid)),
                ("Expired", filteredCache.Count(p => p.Status == StatusEnum.Expired))
            };
            return data;
        }

    }
}
