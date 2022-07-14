namespace GasStationBot.Domain.Entities.Temp
{
    public class TempUserDataModel
    {

        public string ProviderName { get; set; }

        public string City { get; set; }

        public GasStation SelectedGasStation { get; set; }

        public List<Fuel> SelectedFuels { get; set; }

    }
}
