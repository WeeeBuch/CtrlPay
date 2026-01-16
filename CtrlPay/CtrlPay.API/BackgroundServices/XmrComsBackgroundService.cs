
using CtrlPay.XMR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CtrlPay.API.BackgroundServices
{
    public class XmrComsBackgroundService : BackgroundService
    {
        private readonly ILogger<XmrComsBackgroundService> _logger;

        public XmrComsBackgroundService(ILogger<XmrComsBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transaction Processor started.");
            HttpClient httpClient = new HttpClient();

            string username = "monero";
            string password = "+W4Zz9TsoskgFAPDC9/8vw==";
            string uri = "http://178.213.152.7:28088/json_rpc";

            string auth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{username}:{password}")
                );

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            // ⚠️ DŮLEŽITÉ: Monero nemá rádo HTTP/2
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for pending transactions...");

                try
                {
                    await AccountComs.Synchronize(httpClient,uri, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Monero RPC failed");
                    await Task.Delay(5000, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("Transaction Processor stopping.");
        }
    }
}
