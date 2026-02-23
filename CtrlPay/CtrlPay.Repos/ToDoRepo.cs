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
        return list.OrderByDescending(t => t.Timestamp).ToList();
    }
}
