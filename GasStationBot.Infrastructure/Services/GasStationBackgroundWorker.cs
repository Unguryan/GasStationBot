using GasStationBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GasStationBot.Infrastructure.Services
{
    public class GasStationBackgroundWorker : IGasStationBackgroundWorker
    {

        private readonly ILogger<GasStationBackgroundWorker> _logger;

        public GasStationBackgroundWorker(ILogger<GasStationBackgroundWorker> logger)
        {
            _logger = logger;
        }

        private int executionCount = 0;

        public async Task DoWork(IServiceProvider services, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = services.CreateScope())
                {
                    var notifyUserService = scope.ServiceProvider.GetRequiredService<INotifyUserService>();
                    await notifyUserService.NotifyAllUsers();
                }

                executionCount++;
                _logger.LogInformation($"Notify User: {executionCount}");

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }
    }
}
