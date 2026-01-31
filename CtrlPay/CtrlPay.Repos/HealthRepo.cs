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
                await Task.Delay(5000);

                return string.IsNullOrWhiteSpace(connString);
            }
            #endregion

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            using HttpClient client = new(handler);
            string uri = $"{connString}/health/api";

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(uri);
            }
            catch (Exception)
            {
                return false;
            }

            if (response == null)
            {
                return false;
            }

            return true;
        }
    }
}
