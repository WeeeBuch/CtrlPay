
using CtrlPay.Core;

namespace CtrlPay.API.BackgroundServices
{
    public class PaymentProcessingBackgroundService : BackgroundService
    {
        private readonly ILogger<XmrComsBackgroundService> _logger;

        public PaymentProcessingBackgroundService(ILogger<XmrComsBackgroundService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Payment processing starting");
                try
                {
                    await PaymentProcessing.PairOneTimePayment(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Payment processing failed");
                    Task.Delay(5000, stoppingToken).Wait(stoppingToken);
                }
                _logger.LogInformation("Payment processing done");
                Task.Delay(TimeSpan.FromSeconds(30), stoppingToken).Wait(stoppingToken);
            }
        }
    }
}