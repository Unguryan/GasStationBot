namespace GasStationBot.Application.Services
{
    public interface IGasStationBackgroundWorker
    {
        Task DoWork(IServiceProvider services, CancellationToken cancellationToken);
    }
}
