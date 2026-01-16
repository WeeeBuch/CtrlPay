
using CtrlPay.Core;
using CtrlPay.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly MoneroRpcOptions _rpcOptions;

        public XmrComsBackgroundService(ILogger<XmrComsBackgroundService> logger, IOptions<MoneroRpcOptions> rpcOptions)
        {
            _logger = logger;
            _rpcOptions = rpcOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("XMR communication process starting");

            string username = _rpcOptions.Username;
            string password = _rpcOptions.Password;
            string uri = $"http://{_rpcOptions.Host}:{_rpcOptions.Port}/json_rpc";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                PreAuthenticate = false, // u Digest MUSÍ být false
                UseProxy = false
            };

            using var httpClient = new HttpClient(handler);

            // Monero / jiné RPC často vyžaduje HTTP/1.1
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Synchronizing accounts");

                try
                {
                    await XMRComs.SynchronizeAccounts(httpClient,uri, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Monero RPC accounts sync failed");
                    await Task.Delay(5000, stoppingToken);
                }
                _logger.LogInformation("Synchronizing accounts done");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("XMR communication process stopping.");
        }
    }
}
