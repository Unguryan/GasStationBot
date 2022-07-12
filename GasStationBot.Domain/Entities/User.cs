namespace GasStationBot.Domain.Entities
{
    public class User
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public List<GasStation> GasStations { get; set; }

    }
}
