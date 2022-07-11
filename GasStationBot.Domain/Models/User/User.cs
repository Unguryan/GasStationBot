namespace GasStationBot.Domain.Models.User
{
    public class User
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public List<UserGasStation> GasStations { get; set; }

    }
}
