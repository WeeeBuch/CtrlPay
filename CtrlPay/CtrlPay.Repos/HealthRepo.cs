using CtrlPay.Entities;
using CtrlPay.Repos;
using CtrlPay.Repos.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Repos
{
    public class HealthRepo
    {
        public static async Task<bool> TestConnectionToAPI(string connString)
        {
            #region Debug
            if (DebugMode.SkipApiConnectionTest)
            {
                await Task.Delay(2000);
                return !string.IsNullOrWhiteSpace(connString);
            }
            #endregion

            try
            {
                string oldCreds = Credentials.BaseUri;

                Credentials.BaseUri = connString;

                var result = await HttpWorker.HttpGet("/health/api");

                Credentials.BaseUri = oldCreds;
                return result != null;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Health check connection test failed in Repo.", ex);
                return false;
            }
        }
    }
}
