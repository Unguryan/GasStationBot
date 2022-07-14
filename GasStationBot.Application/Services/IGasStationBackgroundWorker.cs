namespace GasStationBot.Application.Services
{
    public interface IGasStationBackgroundWorker
    {
        Task DoWork(INotifyUserService notifyUserService, CancellationToken cancellationToken);
    }
}
