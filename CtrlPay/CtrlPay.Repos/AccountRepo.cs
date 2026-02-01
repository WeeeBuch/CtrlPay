using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class AccountRepo
    {
        public static async Task<string> GetOneTimeAddressForLoyalCustomer(FrontendTransactionDTO transaction)
        {
            AppLogger.Info("Generating one-time address for loyal customer...");
            #region Debug

            if (DebugMode.SkipOneTimeAddressGeneration)
            {
                return "one_time_address_debug";
            }

            #endregion

            int paymentId = transaction.Id;

            object payload = new
            {
                paymentId = paymentId
            };

            string? response = await HttpWorker.HttpPost("/account/one-time-address", payload, true, new CancellationToken());
            if (response == null)
            {
                AppLogger.Error("Failed to get one-time address for loyal customer: No response from server.");
                throw new Exception("No response from server.");
            }

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ReturnModel<string>>(response, serializerOptions);

            return result!.Body;
        }
    }
}
