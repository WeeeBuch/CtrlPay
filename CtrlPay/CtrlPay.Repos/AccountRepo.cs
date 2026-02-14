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
        public static string CreditAddress { get; private set; } = string.Empty;
        public static DateTime CreditAddressLastUpdated { get; private set; } = DateTime.MinValue;
        public static async Task<string> GetCreditAddress()
        {
            if (CreditAddress == string.Empty || CreditAddressLastUpdated != DateTime.UtcNow)
            {
                AppLogger.Info("Credit address is missing or outdated. Updating credit address...");
                CreditAddress = await UpdateCreditAddress();
                CreditAddressLastUpdated = DateTime.UtcNow;
            }
            AppLogger.Info("Returning cached credit address");
            return CreditAddress;
        }
        private static async Task<string> UpdateCreditAddress()
        {
            if (DebugMode.SkipCreditAddressLogic) return "DEBUG_address_for_credits";

            string? response = await HttpWorker.HttpGet("api/account/credit-address", true, new CancellationToken());
            if (response == null)
            {
                AppLogger.Error("Failed to get credit address for loyal customer: No response from server.");
                throw new Exception("No response from server.");
            }

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ReturnModel<string>>(response, serializerOptions);

            return result!.Body;
        }
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

            string? response = await HttpWorker.HttpPost("api/account/one-time-address", payload, true, new CancellationToken());
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
