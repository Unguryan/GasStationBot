namespace GasStationBot.Domain.Models.GasStation
{
    public record GasStation
    {
        public string Provider { get; init; }

        public string City { get; init; }

        public string Address { get; init; }

        public List<Fuel> Fuels { get; init; }

    }
}
