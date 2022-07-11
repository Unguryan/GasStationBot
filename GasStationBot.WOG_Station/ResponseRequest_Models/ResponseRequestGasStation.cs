namespace GasStationBot.WOG_Station.ResponseRequest_Models
{
    public class ResponseRequestGasStation
    {
        public int Id { get; set; }

        public string Link { get; set; }

        public string City { get; set; }

        //address
        public string Name { get; set; }

        public string WorkDescription { get; set; }

        public List<ResponseRequestFuel> Fuels { get; set; }

    }
}
