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
    public static async Task PromoteCustomer(FrontendCustomerDTO dto)
    {
        AppLogger.Info($"Promoting customer {dto.Id} to Loyal Customer...");

        if (DebugMode.FakePromote)
        {
            dto.IsLoyal = true;
            return;
        }

        string? json = await HttpWorker.HttpPost($"api/customers/promote/{dto.Id}", "", true, default);

        if (string.IsNullOrWhiteSpace(json))
        {
            AppLogger.Warning($"Promote response was NULL.");
            return;
        }

        dto.IsLoyal = true;
        AppLogger.Info($"Customer {dto.Id} promoted successfully.");
    }
}
