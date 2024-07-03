using ERPCronJob.Interfaces;

namespace ERPCronJob.Services.Job
{
    public class ERPRequeryJob:CronJobService
    {
        private readonly ILogger<ERPRequeryJob> _logger;
        private readonly IServiceProvider _serviceProvider;



        public ERPRequeryJob(IScheduleConfig<ERPRequeryJob> config, ILogger<ERPRequeryJob> logger, IServiceProvider serviceProvider)
           : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            //_data = data;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ERPRequeryServiceJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ERPRequeryServiceJob is working.");

            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IERPRequeryService>();
            await svc.DoWork(cancellationToken);
        }
    }
}
