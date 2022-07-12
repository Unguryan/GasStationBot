namespace GasStationBot.Domain.Entities
{
    public record Fuel
    {

        public string Name { get; init; }

        public FuelType FuelType { get; init; }

        public bool IsExist => !StateOfFuel.Any(x => x.Equals(FuelState.NotExist));

        public List<FuelState> StateOfFuel { get; init; }
    }
}
