using CtrlPay.XMR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Core
{
    public static class XMRComs
    {
        public static async Task SynchronizeAccounts(HttpClient httpClient, string uri, CancellationToken cancellationToken)
        {
            await AccountComs.Synchronize(httpClient, uri, cancellationToken);
        }
    }
}
