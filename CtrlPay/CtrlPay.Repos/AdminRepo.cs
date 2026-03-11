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
        public static List<FrontendUserDTO> UserCache;


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

        public static async Task<List<FrontendUserDTO>> GetAdminUsers(CancellationToken ct = default) => UserCache.Where(u => u.Role == Role.Admin).ToList();

        public static async Task UpdateAdminUser(FrontendUserDTO user)
        {
            AppLogger.Info($"Updating admin user {user.Username}...");
#if DEBUG
            if (DebugMode.MockAdminUsers)
            {
                await Task.Delay(500);
                UserCache.Add(user);
                return;
            }
#endif
        }

    }
}
