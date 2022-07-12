using GasStationBot.Domain.Entities;

namespace GasStationBot.WOG_Station.Core
{
    public static class WOG_Constants
    {
        public static string BaseUrl => "https://api.wog.ua/fuel_stations";

        public static List<string> Fuels => new()
        {
            "М100",
            "A100",
            "М98",
            "A98",
            "М95",
            "А95",
            "М92",
            "А92",
            "МДП",
            "ДП",
            "ГАЗ"
        };

        public static Dictionary<string, FuelType> FuelTypes
           => new()
           {
                { "М100", FuelType.Petrol_100_Plus },
                { "A100", FuelType.Petrol_100 },
                { "М98", FuelType.Petrol_98_Plus },
                { "A98", FuelType.Petrol_98 },
                { "М95", FuelType.Petrol_95_Plus },
                { "А95", FuelType.Petrol_95 },
                { "М92", FuelType.Petrol_92_Plus },
                { "А92", FuelType.Petrol_92 },
                { "МДП", FuelType.Diesel_Plus },
                { "ДП", FuelType.Diesel },
                { "ГАЗ", FuelType.Gas }
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
