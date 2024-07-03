using ERPCronJob.Interfaces;

namespace ERPCronJob.Services.Job
{
    public class PaymentGateWayRequeryJob:CronJobService
    {
        private readonly ILogger<PaymentGateWayRequeryJob> _logger;
        private readonly IServiceProvider _serviceProvider;



        public PaymentGateWayRequeryJob(IScheduleConfig<PaymentGateWayRequeryJob> config, ILogger<PaymentGateWayRequeryJob> logger, IServiceProvider serviceProvider)
           : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            //_data = data;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentGateWayRequeryJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} PaymentGateWayRequeryJob is working.");

            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IPaymentGateWayRequeryService>();
            await svc.DoWork(cancellationToken);
        }
    }
}
