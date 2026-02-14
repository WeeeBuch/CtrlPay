using CtrlPay.Core;

public class PaymentProcessingBackgroundService : BackgroundService
{
    private readonly ILogger<PaymentProcessingBackgroundService> _logger;

    public PaymentProcessingBackgroundService(ILogger<PaymentProcessingBackgroundService> logger)
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
                await PaymentProcessing.CompleteTransactionsToPrimaryAddress(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed");
                await Task.Delay(5000, stoppingToken);
            }

            _logger.LogInformation("Payment processing done");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
