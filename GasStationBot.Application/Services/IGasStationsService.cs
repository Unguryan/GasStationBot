using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services
{
    public interface IGasStationsService
    {
        string GasStationName { get; }

        Task<IEnumerable<GasStation>> GetGasStations();

        Task<IEnumerable<GasStation>> GetGasStationsWithoutAdditionalData();
    }
}
