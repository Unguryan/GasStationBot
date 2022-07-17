using GasStationBot.Domain.Entities;

namespace GasStationBot.WOG_Station.Core
{
    public static class OKKO_Constants
    {
        public static string BaseUrl => "https://www.okko.ua/api/uk/type/gas_stations?";

        public static List<string> Fuels => new()
        {
            "М95",
            "А95",
            "МДП",
            "ДП",
            "ГАЗ"
        };

        

        public static Dictionary<string, FuelType> FuelTypes
           => new()
           {
                { "PULLS 95", FuelType.Petrol_95_Plus },
                { "A-95", FuelType.Petrol_95 },
                { "PULLS Diesel", FuelType.Diesel_Plus },
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
