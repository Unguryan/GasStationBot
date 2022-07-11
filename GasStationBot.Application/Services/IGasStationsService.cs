using GasStationBot.Domain.Models.GasStation;

namespace GasStationBot.Application.Services
{
    public interface IGasStationsService
    {

        Task<IEnumerable<GasStation>> GetGasStations();

    }
}
