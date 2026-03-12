using CtrlPay.Entities;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class AdminRepo
    {
        private static List<FrontendUserDTO> UserCache;
        private static DateTime LastUpdated { get; set; } = DateTime.MinValue;
        private static JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };


        public static async Task UpdateUserCacheFromApi()
        {
#if DEBUG
            if (DebugMode.MockAdminUsers)
            {
                UserCache = [
                    new() { Id = 1, Username = "admin_karel", Role = Role.Admin, TwoFactorEnabled = true },
                    new() { Id = 2, Username = "uctarni_jana", Role = Role.Accountant, TwoFactorEnabled = false },
                    new() { Id = 3, Username = "zamestnanec_pepa", Role = Role.Employee, TwoFactorEnabled = false }
                ];

                return;
            }
#endif
            AppLogger.Info($"Getting json from API...");
            string? json = await HttpWorker.HttpGet("api/admin/users", true, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Get response was NULL.");
                return;
            }

            try
            {
                AppLogger.Info($"Deserializing response...");
                var result = JsonSerializer.Deserialize<ReturnModel<List<UserApiDTO>>>(json, SerializerOptions);

                // Pokud je Body null, použijeme prázdný list, aby to nespadlo
                var apiList = result?.Body ?? [];

                // Atomická aktualizace - vytvoříme nový list a pak ho přiřadíme
                // Kdyby někdo zrovna četl Cache, aplikace nespadne
                UserCache = [.. apiList.Select(u => new FrontendUserDTO(u))];
                LastUpdated = DateTime.UtcNow;
                AppLogger.Info($"Cached Users updated at {LastUpdated}.");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Users list parsing failed.", ex);
            }
        }

        public static List<FrontendUserDTO> GetUsers() => UserCache;

        public static async Task UpdateUser(FrontendUserDTO user)
        {
            AppLogger.Info($"Updating/Creating user {user.Username}...");
#if DEBUG
            if (DebugMode.MockAdminUsers)
            {
                await Task.Delay(500);
                UserCache.Add(user);
                return;
            }
#endif

            int id = UserCache.FindIndex(c => c.Id == user.Id);
            if (id == -1)
            {
                AppLogger.Info($"Adding customer to API...");
                string? json = await HttpWorker.HttpPost($"api/admin/users/create", user.ToApiDTO(), true, default);
                if (string.IsNullOrWhiteSpace(json))
                {
                    AppLogger.Warning($"Get response was NULL.");
                    return;
                }
                UserCache.Add(user);
            }
            else
            {
                AppLogger.Info($"Updating customer in API...");
                string? json = await HttpWorker.HttpPost($"api/admin/users/update", user.ToApiDTO(), true, default);
                if (string.IsNullOrWhiteSpace(json))
                {
                    AppLogger.Warning($"Get response was NULL.");
                    return;
                }
                UserCache[id] = user;
            }
        }

        public static async Task DeleteUser(FrontendUserDTO user)
        {
            AppLogger.Info($"Deleting user {user.Username}...");

            string? json = await HttpWorker.HttpDelete($"api/admin/users/delete/{user.Id}", "", true, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                AppLogger.Warning($"Get response was NULL.");
                return;
            }
            UserCache.Remove(user);
        }
    }
}
