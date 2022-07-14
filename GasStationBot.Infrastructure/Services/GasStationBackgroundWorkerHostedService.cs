using GasStationBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GasStationBot.Infrastructure.Services
{
    public class GasStationBackgroundWorkerHostedService : BackgroundService
    {
        
        private readonly IServiceProvider _services;

        private readonly ILogger<GasStationBackgroundWorkerHostedService> _logger;

        public GasStationBackgroundWorkerHostedService(IServiceProvider services,
                                                       ILogger<GasStationBackgroundWorkerHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background worker have started work");

            await DoWork(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background worker have ended work");
            return base.StopAsync(cancellationToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using(var scope = _services.CreateScope())
            {
                var backgroundWorker = scope.ServiceProvider.GetRequiredService<IGasStationBackgroundWorker>();
                await backgroundWorker.DoWork(scope.ServiceProvider, stoppingToken);
            }
        }
    }
}
