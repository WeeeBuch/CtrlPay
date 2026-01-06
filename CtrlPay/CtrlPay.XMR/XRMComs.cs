using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;

namespace CtrlPay.XMR
{
    public class XRMComs
    {
        public HttpClient HttpClient { get; set; }
        public XRMComs()
        {
            HttpClient = new HttpClient();
        }

        public async Task<string> FetchTransactions()
        {
            HttpResponseMessage message = await HttpClient.PostAsJsonAsync("",new { Ahoj = "cus"});

            return await message.Content.ReadFromJsonAsync<string>();
        }
    }
}
