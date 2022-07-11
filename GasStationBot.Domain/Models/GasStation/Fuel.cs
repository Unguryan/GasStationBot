using GasStationBot.Domain.Models.User;

namespace GasStationBot.Domain.Models.GasStation
{
    public record Fuel
    {

        public string Name { get; init; }

        public bool IsExist => !StateOfFuel.Any(x => x.Equals(FuelState.NotExist));

        public List<FuelState> StateOfFuel { get; init; }
    }
}
