namespace GasStationBot.Domain.Models.User
{
    public class UserGasStation
    {

        public string Provider { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public List<FuelType> Fuels { get; set; }

    }
}
