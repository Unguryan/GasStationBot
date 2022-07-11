using GasStationBot.Domain.Models.GasStation;

namespace GasStationBot.WOG_Station.Core
{
    public static class WOG_Constants
    {
        public static string BaseUrl => "https://api.wog.ua/fuel_stations";

        public static List<string> Fuels => new()
        {
            "М95",
            "А95",
            "М92",
            "А92",
            "МДП",
            "ДП",
            "ГАЗ"
        };

        public static Dictionary<string, FuelState> FuelStates
            => new()
            {
                { "Пальне відсутнє", FuelState.NotExist },
                { "Талони", FuelState.Talons },
                { "Тільки спецтранспорт", FuelState.OnlyForSpecVehicles },
                { "Готівка, банк.картки", FuelState.Exist }
            };
    }
}
