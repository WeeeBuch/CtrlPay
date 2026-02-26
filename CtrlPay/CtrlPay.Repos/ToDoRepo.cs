using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public static class ToDoRepo
{
    public static List<FrontendUserDTO> Cache = [];

    public static async Task<List<FrontendUserDTO>> GetAdminUsers(CancellationToken ct = default)
    {
        if (DebugMode.MockAdminUsers)
        {
            Cache = [
                new() { Id = 1, Username = "admin_karel", Role = Role.Admin, TwoFactorEnabled = true },
                new() { Id = 2, Username = "uctarni_jana", Role = Role.Accountant, TwoFactorEnabled = false },
                new() { Id = 3, Username = "zamestnanec_pepa", Role = Role.Employee, TwoFactorEnabled = false }
            ];

            return Cache;
            
        }

        // string? json = await HttpWorker.HttpGet("api/admin/users", true, ct);
        return [];
    }

    public static async Task UpdateAdminUser(FrontendUserDTO user)
    {
        AppLogger.Info($"Updating admin user {user.Username}...");
        
        if (DebugMode.MockAdminUsers)
        {
            await Task.Delay(500); 
            Cache.Add(user);
            return;
        }
    }

    public static AccountantDashboardSummaryDTO GetAccountantDashboardSummary()
    {
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

        // TODO: Propojit s reálným API v AccountantPaymentRepo
        throw new NotImplementedException();
    }

    public static List<AccountantTransactionDTO> GetMockAccountantTransactions()
    {
        var rng = new Random();
        var customers = new[] { "Alice Corp.", "Bob's Burgers", "Charlie Chap", "Delta Force", "Echo Base", "Cyberdyne Systems", "Initech", "Umbrella Corp" };
        var types = new[] { TransactionTypeEnum.Incoming, TransactionTypeEnum.Outgoing };
        var statuses = new[] { StatusEnum.Completed, StatusEnum.Pending, StatusEnum.Cancelled, StatusEnum.Confirmed, StatusEnum.Overpaid };

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

    public static List<AccountantTransactionDTO> GetAccountantTransactions()
    {
        if (DebugMode.MockAccountantTransactions) return GetMockAccountantTransactions();

        // TODO: Karele toto
        throw new NotImplementedException();
    }
}
