using CtrlPay.Entities;
using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class AdminRepo
    {
        private static List<FrontendUserDTO> UserCache;


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
            string? json = await HttpWorker.HttpGet("api/admin/users", true, default);
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
                string? json = await HttpWorker.HttpPost($"api/admin/users/edit", user.ToApiDTO(), true, default);
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
