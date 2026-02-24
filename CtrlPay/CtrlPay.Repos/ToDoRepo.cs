using CtrlPay.Entities;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos;

public static class ToDoRepo
{
    public static async Task<List<FrontendUserDTO>> GetAdminUsers(CancellationToken ct = default)
    {
        if (DebugMode.MockAdminUsers)
        {
            return new List<FrontendUserDTO>
            {
                new FrontendUserDTO { Id = 1, Username = "admin_karel", Role = Role.Admin, TwoFactorEnabled = true },
                new FrontendUserDTO { Id = 2, Username = "uctarni_jana", Role = Role.Accountant, TwoFactorEnabled = false },
                new FrontendUserDTO { Id = 3, Username = "zamestnanec_pepa", Role = Role.Employee, TwoFactorEnabled = false }
            };
        }

        // string? json = await HttpWorker.HttpGet("api/admin/users", true, ct);
        return new List<FrontendUserDTO>();
    }

    public static async Task UpdateAdminUser(FrontendUserDTO user)
    {
        AppLogger.Info($"Updating admin user {user.Username}...");
        
        if (DebugMode.MockAdminUsers)
        {
            await Task.Delay(500); 
            return;
        }
    }
}
